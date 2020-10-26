using System;
using System.Collections;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Prism.Events;
using Prism.Commands;
using KEI.Infrastructure;
using KEI.Infrastructure.Prism;
using KEI.Infrastructure.Screen;
using KEI.UI.Wpf;
using KEI.UI.Wpf.Hotkey;
using Application.Production.Views;
using Application.Production.Screen;
using Prism.Ioc;

namespace Application.Production.ViewModels
{
    [Screen(DisplayName = @"Manage Screens",
            Icon = Icon.None16x,
            ScreenName = nameof(ScreenConfigEditor),
            ParentName = nameof(ConfigScreen))]
    public class ScreenConfigEditorViewModel : BaseScreenViewModel<ScreenConfigEditor>
    {
        #region BaseScreenViewModel Members
        public override string DisplayName { get; set; } = @"Manage Screens";
        public override Icon Icon { get; set; } = Icon.None16x;

        #endregion

        #region Injected Members

        private readonly ScreenConfig _screenConfig;
        private readonly IViewService _viewService;

        #endregion

        #region Constructor
        public ScreenConfigEditorViewModel(IViewService viewService, ScreenConfig screenConfig, 
            IHotkeyService hotkeyService) : base(hotkeyService)
        {
            _viewService = viewService;
            _screenConfig = screenConfig;


            ActiveScreens = _screenConfig.Config;
            InactiveScreens = _screenConfig.InactiveScreens;

            Commands.Add(new ScreenCommand
            {
                DisplayName = "Update",
                Command = RefreshConfigCommand,
                Icon = Icon.Refresh16x
            });
            Commands.Add(new ScreenCommand
            {
                DisplayName = "Save",
                Command = SaveConfigCommand,
                Icon = Icon.Save16x
            });
        }

        #endregion

        #region Properties

        private ObservableCollection<ScreenInfo> activeScreens = null;
        public ObservableCollection<ScreenInfo> ActiveScreens
        {
            get => activeScreens;
            set => SetProperty(ref activeScreens, value);
        }

        private ObservableCollection<ScreenInfo> inactiveScreens = null;
        public ObservableCollection<ScreenInfo> InactiveScreens
        {
            get => inactiveScreens;
            set => SetProperty(ref inactiveScreens, value);
        }

        #endregion

        #region Add Screens

        private DelegateCommand<IList> addScreenCommand;
        public DelegateCommand<IList> AddScreenCommand =>
            addScreenCommand ?? (addScreenCommand = new DelegateCommand<IList>(ExecuteAddScreenCommand));
        private void ExecuteAddScreenCommand(IList screens)
        {
            var arrScreens = Array.CreateInstance(typeof(ScreenInfo), screens.Count);
            screens.CopyTo(arrScreens, 0);

            foreach (ScreenInfo screen in arrScreens)
            {
                ;
                ActiveScreens.Add(screen);
                InactiveScreens.Remove(screen);
            }

            RaisePropertyChanged(nameof(InactiveScreens));
        }

        #endregion

        #region Remove Screens

        private DelegateCommand<IList> removeScreenCommand;
        public DelegateCommand<IList> RemoveScreenCommand =>
            removeScreenCommand ?? (removeScreenCommand = new DelegateCommand<IList>(ExecuteRemoveScreenCommand));
        private void ExecuteRemoveScreenCommand(IList screens)
        {
            var arrScreens = Array.CreateInstance(typeof(ScreenInfo), screens.Count);
            screens.CopyTo(arrScreens, 0);

            foreach (ScreenInfo screen in arrScreens)
            {
                if (!screen.IsMandatory)
                {
                    ActiveScreens.Remove(screen);
                    InactiveScreens.Add(screen);
                }
            }

            RaisePropertyChanged(nameof(InactiveScreens));
        }

        #endregion

        #region Save

        private DelegateCommand saveConfigCommand;
        public DelegateCommand SaveConfigCommand =>
            saveConfigCommand ?? (saveConfigCommand = new DelegateCommand(ExecuteSaveConfigCommand));
        private void ExecuteSaveConfigCommand()
        {
            if (_viewService.Prompt($"Are you sure want overwrite \"{_screenConfig.ConfigPath}\" ?", PromptOptions.YesNo) == PromptResult.Yes)
            {
                ExecuteRefreshConfigCommand();
                _screenConfig.StoreConfig();
            }
        }

        #endregion

        #region Refresh / Update

        private DelegateCommand refreshConfigCommand;
        public DelegateCommand RefreshConfigCommand =>
            refreshConfigCommand ?? (refreshConfigCommand = new DelegateCommand(ExecuteRefreshConfigCommand));
        private void ExecuteRefreshConfigCommand()
        {
            _screenConfig.Config = ActiveScreens;
            ContainerLocator.Container.Resolve<IEventAggregator>().GetEvent<ScreenConfigUpdatedEvent>().Publish(ActiveScreens);
        }

        #endregion


        public override void RegisterHotkeys()
        {
            // Save ScreenConfig Command
            _hotkeyService.AddReadonlyGesture(GestureCache.GetGesture($"{ScreenName}.Save",
                SaveConfigCommand,
                null,
                Key.S,
                ModifierKeys.Control));
        }

        public override void UnregisterHotkeys()
        {
            // Save ScreenConfig Command
            _hotkeyService.RemoveGesture($"{_assemblyName}.{ScreenName}.Save");
        }
    }
}
