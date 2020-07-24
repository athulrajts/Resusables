using System.Linq;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Prism.Events;
using Prism.Regions;
using Prism.Commands;
using CommonServiceLocator;
using KEI.Infrastructure;
using KEI.Infrastructure.Prism;
using KEI.Infrastructure.Screen;
using KEI.Infrastructure.Configuration;
using KEI.UI.Wpf;
using Application.Core;
using Application.UI;
using KEI.UI.Wpf.Hotkey;
using System.Reflection;
using Application.Production.Screen;
using System;

namespace Application.Production
{
    [RegisterSingleton(NeedResolve = false)]
    public class ProductionViewModel : BaseViewModel<ProductionView>, INavigationAware
    {
        #region Injected Members

        private readonly IRegionManager regionManager;
        private readonly IDataContainer generalPreferences;
        private readonly IViewService _viewService;
        private readonly IHotkeyService _hotkeyService;
        private readonly string _assemblyName;

        #endregion

        #region Constructor

        public ProductionViewModel(IRegionManager rm, GeneralPreferences gp,
            IViewService viewService, IHotkeyService hotkeyService)
        {
            regionManager = rm;
            _viewService = viewService;
            _hotkeyService = hotkeyService;
            _assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

            ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<ScreenConfigUpdatedEvent>().Subscribe((screens) => PopulateScreens(screens));

            generalPreferences = gp.Config;

            AddPropertyObserver(generalPreferences, "ShowCommandPanelOnLeftSide");
            UpdateCommandBarLocation();
        }

        #endregion

        #region Overridden Members
        protected override void ProcessPropertyChanged(object sender, string property)
        {
            switch (property)
            {
                case "ShowCommandPanelOnLeftSide":
                    UpdateCommandBarLocation();
                    break;
            }
        }

        #endregion

        #region Properties

        private string currentScreen;
        private ObservableCollection<IScreenViewModel> screens = new ObservableCollection<IScreenViewModel>();
        private int commandsGridColumn = 2;
        private int subViewsGridColumn = 0;

        /// <summary>
        /// ViewModel of currently displaying screen
        /// </summary>
        public IScreenViewModel CurrentScreenViewModel { get; set; }

        /// <summary>
        /// Name of Currently Displaying Screen
        /// </summary>
        public string CurrentScreen
        {
            get => currentScreen;
            set => SetProperty(ref currentScreen, value, OnScreenChanged);
        }

        /// <summary>
        /// Collection of Available Screens, that are configured in 
        /// <see cref="ScreenConfig"/>
        /// </summary>
        public ObservableCollection<IScreenViewModel> Screens
        {
            get => screens;
            set => SetProperty(ref screens, value);
        }

        /// <summary>
        /// Location of Commands Panel
        /// Needed when we need to swap location of Command and SubViews Panels
        /// <see cref="UpdateCommandBarLocation"/>
        /// </summary>
        public int CommandsGridColumn
        {
            get => commandsGridColumn;
            set => SetProperty(ref commandsGridColumn, value);
        }

        /// <summary>
        /// Location of SubViews Panels
        /// Needed when we need to swap location of Command and SubViews Panels
        /// <see cref="UpdateCommandBarLocation"/>
        /// </summary>
        public int SubViewsGridColum
        {
            get => subViewsGridColumn;
            set => SetProperty(ref subViewsGridColumn, value);
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Pouplate the Navigation Pane with buttons
        /// </summary>
        /// <param name="screens">Screens to show, if null, will retrieve value from <see cref="ScreenConfig"/></param>
        public void PopulateScreens(ObservableCollection<ScreenInfo> screens = null)
        {
            Screens.Clear();

            var screenInfo = screens == null ? ServiceLocator.Current.GetInstance<ScreenConfig>().Config : screens;

            foreach (var screen in screenInfo.Where(x => string.IsNullOrEmpty(x.ParentScreenName)))
            {
                if (BaseScreenViewModel.Instances.Find(x => x.ScreenName == screen.ScreenName) is IScreenViewModel vm)
                {
                    vm.SubViews.Clear();
                    vm.Icon = screen.Icon;
                    vm.DisplayName = screen.DisplayName;

                    var subViews = screenInfo.Where(x => x.ParentScreenName == vm.ScreenName);
                    foreach (var view in subViews)
                    {
                        if (BaseScreenViewModel.Instances.Find(x => x.ScreenName == view.ScreenName) is IScreenViewModel svm)
                        {
                            svm.Icon = view.Icon;
                            svm.DisplayName = view.DisplayName;
                            svm.ParentScreenViewModel = vm;
                            svm.IsChildScreen = true;
                            vm.SubViews.Add(svm);
                        }
                    }

                    Screens.Add(vm);
                }
            }

            if (Screens.FirstOrDefault((x) => x.IsSelected) is IScreenViewModel si)
                CurrentScreen = si.ScreenName;
            else
                CurrentScreen = Screens.ElementAtOrDefault(0)?.ScreenName;

            RegisterNavigationHotkeys();
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Swap the position of Command and SubViews Panel based on the value of "ShowCommandPanelOnLeftSide"
        /// </summary>
        private void UpdateCommandBarLocation()
        {
            bool val = false;
            generalPreferences.Get("ShowCommandPanelOnLeftSide", ref val);
            if (val)
            {
                CommandsGridColumn = 0;
                SubViewsGridColum = 2;
            }
            else
            {
                CommandsGridColumn = 2;
                SubViewsGridColum = 0;
            }
        }

        /// <summary>
        /// Change Displaying Screen
        /// </summary>
        private void OnScreenChanged()
        {
            if (string.IsNullOrEmpty(currentScreen))
                return;

            CurrentScreenViewModel = Screens.FirstOrDefault(screen => screen.ScreenName == currentScreen);

            regionManager.RequestNavigate(RegionNames.ProductionShell, CurrentScreenViewModel.CurrentSubViewModel.ScreenName, OnNavigated);

            RaisePropertyChanged(nameof(CurrentScreenViewModel));
        }

        /// <summary>
        /// For Debugging when navigations fails
        /// </summary>
        /// <param name="obj">Navigation Result</param>
        private void OnNavigated(NavigationResult obj)
        {
            ;
        }

        private void RegisterNavigationHotkeys()
        {
            for (int i = 0; i < Math.Min(10, Screens.Count); i++)
            {
                var commandName = $"Nav.{Screens[i].DisplayName}";

                _hotkeyService.RemoveGesture($"{_assemblyName}.{commandName}");

                _hotkeyService.AddReadonlyGesture(GestureCache.GetGesture(commandName,
                    SwitchScreenCommand,
                    Screens[i].ScreenName,
                    Key.D1 + i,
                    ModifierKeys.Control));
            }
        }

        #endregion;

        #region Switch Screen

        private DelegateCommand<string> switchScreenCommand;
        public DelegateCommand<string> SwitchScreenCommand =>
            switchScreenCommand ?? (switchScreenCommand = new DelegateCommand<string>(ExecuteSwitchScreenCommand));
        void ExecuteSwitchScreenCommand(string viewName) => CurrentScreen = viewName;

        #endregion

        #region INavigationAware Members

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            //Register Main Navigation Hotkeys
            RegisterNavigationHotkeys();

            //Register Subviews
            if (CurrentScreenViewModel.SubViews.Count > 0)
            {
                // Registering command to self
                _hotkeyService.AddReadonlyGesture(GestureCache.GetGesture($"Nav.{CurrentScreenViewModel.DisplayName}.Home",
                    CurrentScreenViewModel.NavigateSubViewCommand,
                    CurrentScreenViewModel.ScreenName,
                    Key.D1,
                    ModifierKeys.Shift));

                // Register subviews
                for (int i = 0; i < Math.Min(9, CurrentScreenViewModel.SubViews.Count); i++)
                {
                    _hotkeyService.AddReadonlyGesture(GestureCache.GetGesture($"Nav.{CurrentScreenViewModel.SubViews[i].DisplayName}",
                        CurrentScreenViewModel.SubViews[i].NavigateSubViewCommand,
                        CurrentScreenViewModel.SubViews[i].ScreenName,
                        Key.D2 + i,
                        ModifierKeys.Shift));
                }
            }

            //Register Commands
            for (int i = 0; i < Math.Min(8, CurrentScreenViewModel.CurrentSubViewModel.Commands.Count); i++)
            {
                _hotkeyService.AddReadonlyGesture(GestureCache.GetGesture($"Cmd.{CurrentScreenViewModel.CurrentSubViewModel.Commands[i].DisplayName}",
                    CurrentScreenViewModel.CurrentSubViewModel.Commands[i].Command,
                    CurrentScreenViewModel.CurrentSubViewModel.Commands[i].CommandParameter,
                    Key.F1 + i,
                    ModifierKeys.None));
            }

            //Register View specific commands
            if (CurrentScreenViewModel.CurrentSubViewModel is BaseScreenViewModel bsvm)
            {
                bsvm.RegisterHotkeys();
            }
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            // Unregister Main Screens
            for (int i = 0; i < Math.Min(10, Screens.Count); i++)
            {
                _hotkeyService.RemoveGesture($"{_assemblyName}.Nav{Screens[i].DisplayName}");
            }

            // Unregister Selected Screens Subviews
            if (CurrentScreenViewModel.SubViews.Count > 0)
            {
                // Unregistering command to self
                _hotkeyService.RemoveGesture($"{_assemblyName}.Nav.{CurrentScreenViewModel.DisplayName}.Home");

                // Unregister subviews
                for (int i = 0; i < Math.Min(9, CurrentScreenViewModel.SubViews.Count); i++)
                {
                    _hotkeyService.RemoveGesture($"{_assemblyName}.Nav.{CurrentScreenViewModel.SubViews[i].DisplayName}");
                }
            }

            // Unregister Current Subviews Commands
            for (int i = 0; i < Math.Min(10, CurrentScreenViewModel.CurrentSubViewModel.Commands.Count); i++)
            {
                _hotkeyService.RemoveGesture($"{_assemblyName}.Cmd.{CurrentScreenViewModel.CurrentSubViewModel.Commands[i].DisplayName}");
            }

            // Unregister view specific commands
            if (CurrentScreenViewModel.CurrentSubViewModel is BaseScreenViewModel bsvm)
            {
                bsvm.UnregisterHotkeys();
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        #endregion
    }
}
