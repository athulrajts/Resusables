using System;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Regions;
using Prism.Commands;
using KEI.Infrastructure.Screen;
using KEI.Infrastructure.Helpers;
using KEI.UI.Wpf.Hotkey;
using Application.UI;
using Application.Production.Screen;
using KEI.Infrastructure.Logging;
using Prism.Ioc;

namespace KEI.UI.Wpf
{
    /// <summary>
    /// Base class for ViewModels
    /// Implements <see cref="System.ComponentModel.INotifyPropertyChanged"/>
    /// Implements <see cref="IScreenViewModel"/>
    /// </summary>
    public abstract class BaseScreenViewModel : BaseViewModel, IScreenViewModel, INavigationAware
    {
        /// <summary>
        /// Region Manager instance for navigation purposes
        /// </summary>
        protected static IRegionManager _regionManager => ContainerLocator.Container.Resolve<IRegionManager>();

        /// <summary>
        /// A Collection to keep track of all instances of this class which are bound to views
        /// </summary>
        public static List<BaseScreenViewModel> Instances = new List<BaseScreenViewModel>();

        protected readonly IHotkeyService _hotkeyService;
        protected readonly string _assemblyName;

        #region Constructor

        public BaseScreenViewModel(IHotkeyService hotkeyService)
        {
            _hotkeyService = hotkeyService;
            _assemblyName = GetType().Assembly.GetName().Name;

            /// Keeps a collection of All instances of type <see cref="BaseScreenViewModel{TView}"/>
            /// This is done to populate Navigation bars.
            if (GetType().IsSubclassOfRawGeneric(typeof(BaseScreenViewModel<>)))
            {
                Instances.Add(this);
            }
            CurrentSubViewModel = this;
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// A Collection of Sub Views of this screen
        /// </summary>
        private ObservableCollection<IScreenViewModel> subViews = new ObservableCollection<IScreenViewModel>();
        public ObservableCollection<IScreenViewModel> SubViews
        {
            get => subViews;
            set => SetProperty(ref subViews, value);
        }

        /// <summary>
        /// A Collection of Actions that this screen can do which as exposed via UI
        /// </summary>
        private ObservableCollection<ScreenCommand> commands = new ObservableCollection<ScreenCommand>();
        public ObservableCollection<ScreenCommand> Commands
        {
            get => commands;
            set => SetProperty(ref commands, value);
        }

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }

        /// <summary>
        /// Name to be displayed in Navigation bar
        /// </summary>
        public abstract string DisplayName { get; set; }

        /// <summary>
        /// Icon to be displayed in Navigation bar
        /// </summary>
        public abstract Icon Icon { get; set; }

        public IScreenViewModel CurrentSubViewModel { get; set; }

        public IScreenViewModel ParentScreenViewModel { get; set; }

        public bool IsChildScreen { get; set; }

        #endregion

        #region Navigate Subviews Command

        private DelegateCommand<string> navigateSubViewCommand = null;
        public ICommand NavigateSubViewCommand
        {
            get
            {
                if (navigateSubViewCommand == null)
                {
                    navigateSubViewCommand = new DelegateCommand<string>(NavigateCommand);
                }
                return navigateSubViewCommand;
            }
        }

        private void NavigateCommand(string subViewName)
        {
            _regionManager.RequestNavigate(RegionNames.ProductionShell, subViewName);

            if (subViewName == ScreenName)
            {
                CurrentSubViewModel = this;
                RaisePropertyChanged(nameof(CurrentSubViewModel));
            }
        }

        #endregion

        #region INavigationAware Members

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
            RegisterHotkeys();

            // Register command Hotkeys
            for (int i = 0; i < Math.Min(8, Commands.Count); i++)
            {
                _hotkeyService.AddReadonlyGesture(GestureCache.GetGesture(
                    $"Cmd.{Commands[i].DisplayName}",
                    Commands[i].Command,
                    Commands[i].CommandParameter,
                    Key.F1 + i,
                    ModifierKeys.None));
            }

            // Register sub views hotkeys if parent screen
            if (IsChildScreen == false)
            {
                if (SubViews.Count > 0)
                {
                    _hotkeyService.AddReadonlyGesture(GestureCache.GetGesture(
                        $"Nav.{DisplayName}.Home",
                        NavigateSubViewCommand,
                        ScreenName,
                        Key.D1,
                        ModifierKeys.Shift));
                }

                for (int i = 0; i < Math.Min(9, SubViews.Count); i++)
                {
                    _hotkeyService.AddReadonlyGesture(GestureCache.GetGesture(
                        $"Nav.{SubViews[i].DisplayName}",
                        SubViews[i].NavigateSubViewCommand,
                        SubViews[i].ScreenName,
                        Key.D2 + i,
                        ModifierKeys.Shift));
                } 
            }
            else
            {
                // Select parent if child is selected
                ParentScreenViewModel.IsSelected = true;
            }


            IsSelected = true;
            Logger.Debug($"Navigated to {ScreenName}");
            RaisePropertyChanged(nameof(IsSelected));
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
            UnregisterHotkeys();

            if (SubViews.FirstOrDefault(x=> x.ScreenName == navigationContext.Uri.ToString()) is null)
            {
                IsSelected = false;

                // Unregister Hotkeys when navigating to screens other than child views
                foreach (var command in Commands)
                {
                    _hotkeyService.RemoveGesture($"{_assemblyName}.Cmd.{command.DisplayName}");
                }
                foreach (var subview in SubViews)
                {
                    _hotkeyService.RemoveGesture($"{_assemblyName}.Nav.{subview.DisplayName}");
                }

                if (SubViews.Count > 0)
                {
                    _hotkeyService.RemoveGesture($"{_assemblyName}.Nav.{DisplayName}.Home");
                }

            }
            else
            {
                CurrentSubViewModel = Instances.Find(x => x.ScreenName == navigationContext.Uri.ToString());
                RaisePropertyChanged(nameof(CurrentSubViewModel));
                RaisePropertyChanged(nameof(IsSelected));
            }
        }
        #endregion

        public virtual void RegisterHotkeys()
        {
            return;
        }

        public virtual void UnregisterHotkeys()
        {
            return;
        }

    }

    /// <summary>
    /// Absctract class for ViewModels which are tightly coupled with views
    /// This View-ViewModel pair is automatically registered for navigation.
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    public abstract class BaseScreenViewModel<TView> : BaseScreenViewModel
    {
        public BaseScreenViewModel(IHotkeyService hotkeyService) : base(hotkeyService) { }

        public override string ScreenName => typeof(TView).Name;
    }
}
