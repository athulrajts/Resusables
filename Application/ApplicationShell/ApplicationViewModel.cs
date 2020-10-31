using System.Globalization;
using Application.Core;
using Application.UI;
using KEI.Infrastructure;
using KEI.Infrastructure.Events;
using KEI.Infrastructure.Helpers;
using KEI.Infrastructure.Localizer;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using ApplicationMode = Application.Core.ApplicationMode;

namespace ApplicationShell
{
    public class ApplicationViewModel :  BindableBase, ISystemStatusManager
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger _logger;

        public ApplicationViewModel(IRegionManager regionManger, ILogManager logManager,
            IEventAggregator eventAggregator)
        {
            _regionManager = regionManger;
            _eventAggregator = eventAggregator;
            _logger = logManager.GetLogger();
            CurrentLanguage = GetLang();
        }

        private Language GetLang()
        {
            return (CultureInfo.DefaultThreadCurrentUICulture?.Name) switch
            {
                "ja-JP" => Language.Japanese,
                _ => Language.English,
            };
        }

        private ApplicationMode appMode = ApplicationMode.Production;
        public ApplicationMode ApplicationMode
        {
            get => appMode;
            set
            {
                OnApplicationModeChanged(appMode, value);
                appMode = value;
            }
        }

        public bool InstrumentControlEnabled { get; set; }



        private string currentScreen;
        public string CurrentScreen
        {
            get { return currentScreen; }
            set { SetProperty(ref currentScreen, value); }
        }

        private Language currentLanguage;
        public Language CurrentLanguage
        {
            get { return currentLanguage; }
            set
            {
                if (currentLanguage == value)
                    return;
                currentLanguage = value;
                LocalizationManager.Instance.Reload(currentLanguage.GetEnumDescription());
            }
        }

        private void OnApplicationModeChanged(ApplicationMode oldVal, ApplicationMode newVal)
        {
            _eventAggregator.GetEvent<ApplicationModeChangedEvent>().Publish(new PropertyChangedPayLoad<ApplicationMode>(newVal, oldVal));
            SwitchApplicationMode(newVal);
        }

        private void SwitchApplicationMode(ApplicationMode mode)
        {

            switch (mode)
            {
                case ApplicationMode.Production:
                    _regionManager.RequestNavigate(RegionNames.ApplicationShell, "ProductionView", OnNavigated);
                    break;
                case ApplicationMode.Engineering:
                    _regionManager.RequestNavigate(RegionNames.ApplicationShell, "EngineeringView", OnNavigated);
                    break;
            }
        }

        private void OnNavigated(NavigationResult obj)
        {
            //throw new NotImplementedException();
        }
    }

}
