using System;
using System.Timers;
using System.Windows;
using System.Reflection;
using System.ComponentModel;
using Prism.Mvvm;
using Prism.Commands;
using KEI.Infrastructure;
using KEI.Infrastructure.Prism;
using KEI.Infrastructure.UserManagement;
using KEI.UI.Wpf;
using Application.Core;
using KEI.Infrastructure.Localizer;
using System.Globalization;
using KEI.Infrastructure.Helpers;

namespace Application.Production.ViewModels
{
    [RegisterSingleton]
    public class TitleBarViewModel : BindableBase, IWeakEventListener
    {
        #region Injected Members

        private IUserManager _userManager;
        private IApplicationViewService _viewService;
        private ISystemStatusManager _systemStatusManager;
        public IStringLocalizer Localizer { get; set; }

        #endregion

        #region Private Fields

        private Timer _timer = new Timer(1000);
        private ProductionViewModel _pvm;

        #endregion

        #region Constructor
        public TitleBarViewModel(IUserManager userManager, ProductionViewModel productionViewModel,
            IApplicationViewService viewService, ISystemStatusManager systemStatusManager)
        {
            _userManager = userManager;
            _viewService = viewService;
            _systemStatusManager = systemStatusManager;
            _pvm = productionViewModel;
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        public void AttachedListners()
        {
            /// Hook in to property changed Event of Production View Model and IScreeViewModel
            /// implementations to know when to update the currently active screen name
            /// <see cref="CurrentScreen"/>
            PropertyChangedEventManager.AddListener(_pvm, this, "CurrentScreenViewModel");
            PropertyChangedEventManager.AddListener(_userManager, this, "CurrentUser");
            BaseScreenViewModel.Instances.ForEach(vm =>
            {
                PropertyChangedEventManager.AddListener(vm, this, "CurrentSubViewModel");
            });

            RaisePropertyChanged(nameof(CurrentScreen));
        }
        
  
        ~TitleBarViewModel()
        {
            /// Unscribe from all the property changed events
            PropertyChangedEventManager.RemoveListener(_pvm, this, "CurrentScreenViewModel");
            BaseScreenViewModel.Instances.ForEach(vm =>
            {
                PropertyChangedEventManager.RemoveListener(vm, this, "CurrentSubViewModel");
            });

            Dispose();
        }

        #endregion

        #region Properties
        /// <summary>
        /// Machine Name
        /// </summary>
        public string MachineName => Environment.MachineName;

        /// <summary>
        /// Version name of this Assembly
        /// </summary>
        public string Version => Assembly.GetEntryAssembly().GetName().Version.ToString();

        /// <summary>
        /// Currently Active Screen, If current screen has Sub screens, name of 
        /// subscreen is shown
        /// </summary>
        public string CurrentScreen => _pvm?.CurrentScreenViewModel?.CurrentSubViewModel?.DisplayName;

        /// <summary>
        /// Currently Active User.
        /// </summary>
        public IUser CurrentUser => _userManager?.CurrentUser;

        /// <summary>
        /// Current Time
        /// </summary>
        public DateTime Time => DateTime.Now;
        public string Month => _systemStatusManager.CurrentLanguage == Language.English
            ? Time.ToString("MMM")
            : Time.ToString("MMMM", CultureInfo.CreateSpecificCulture(_systemStatusManager.CurrentLanguage.GetEnumDescription()));

        #endregion

        #region Exit Application

        private DelegateCommand exitApplictionCommand;
        public DelegateCommand ExitApplicationCommand =>
            exitApplictionCommand ?? (exitApplictionCommand = new DelegateCommand(ExecuteExitApplicationCommand));
        private void ExecuteExitApplicationCommand()
        {
            // TODO : Can Exit Check;

            if (_viewService.Prompt("Are you sure you want to exit application ?", PromptOptions.YesNo) == PromptResult.Yes)
                System.Windows.Application.Current.Shutdown();
        }

        #endregion

        #region Switch User

        private DelegateCommand switchUserCommand;
        public DelegateCommand SwitchUserCommand =>
            switchUserCommand ?? (switchUserCommand = new DelegateCommand(ExecuteSwitchUserCommand));
        void ExecuteSwitchUserCommand() => _viewService.SwitchUser();

        #endregion

        #region Info

        private DelegateCommand infoCommand;
        public DelegateCommand InfoCommand =>
            infoCommand ?? (infoCommand = new DelegateCommand(ExecuteInfoCommand));
        private void ExecuteInfoCommand()
        {
            var name = Assembly.GetEntryAssembly().GetName().Name;
            var version = Assembly.GetEntryAssembly().GetName().Version;

            var message = $"Name\t : {name}\nVersion\t : {version}";
            _viewService.Inform(message);
        }

        #endregion

        #region Advanced Setup

        private DelegateCommand advancedSetupCommand;
        public DelegateCommand AdvancedSetupCommand =>
            advancedSetupCommand ?? (advancedSetupCommand = new DelegateCommand(ExecuteAdvancedSetupCommand));
        void ExecuteAdvancedSetupCommand() => _viewService.ShowAdvancedSetup();

        #endregion

        #region Switch To Engineering

        private DelegateCommand switchToEngineeringMode;
        public DelegateCommand SwitchToEngineeringMode =>
            switchToEngineeringMode ?? (switchToEngineeringMode = new DelegateCommand(ExecuteSwitchToEngineeringMode));
        void ExecuteSwitchToEngineeringMode() => _systemStatusManager.ApplicationMode = ApplicationMode.Engineering;

        #endregion

        #region IWeakEventListener Members

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(PropertyChangedEventManager))
            {
                var eArgs = e as PropertyChangedEventArgs;
                string propName = eArgs.PropertyName;

                if (sender is ProductionViewModel)
                {
                    if (propName == "CurrentScreenViewModel")
                    {
                        RaisePropertyChanged(nameof(CurrentScreen));
                        _systemStatusManager.CurrentScreen = _pvm.CurrentScreenViewModel.CurrentSubViewModel.ScreenName;
                    }
                }
                else if (sender is BaseScreenViewModel)
                {
                    if (propName == "CurrentSubViewModel")
                    {
                        RaisePropertyChanged(nameof(CurrentScreen));
                        _systemStatusManager.CurrentScreen = _pvm.CurrentScreenViewModel.CurrentSubViewModel.ScreenName;
                    }
                }
                else if (sender is IUserManager)
                {
                    if (propName == "CurrentUser")
                    {
                        RaisePropertyChanged(nameof(CurrentUser));
                    }
                }

                return true;
            }
            return false;
        }

        #endregion

        #region Events

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            RaisePropertyChanged(nameof(Time));
            RaisePropertyChanged(nameof(Month));
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        #endregion
    }
}
