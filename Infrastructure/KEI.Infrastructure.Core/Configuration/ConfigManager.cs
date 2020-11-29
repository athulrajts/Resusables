using System.Linq;
using System.Collections.ObjectModel;
using Prism.Events;

namespace KEI.Infrastructure.Configuration
{
    public class ConfigManager : IConfigManager
    {
        protected IViewService _viewService;

        #region Constructor
        public ConfigManager(IEventAggregator eventAggregator, IViewService viewService)
        {
            eventAggregator.GetEvent<ConfigLoadedEvent>().Subscribe(config =>
            {
                Configs.Add(config);
            });
            _viewService = viewService;
        }

        #endregion

        #region IConfigManager Members

        public ObservableCollection<IPropertyContainer> Configs { get; set; } = new ObservableCollection<IPropertyContainer>();

        public IPropertyContainer GetConfig(string configFullName)
        {
            var configObj = Configs.FirstOrDefault(x => x.Name == configFullName);

            if (configObj is null)
            {
                _viewService.Error($"DataContainer \"{configFullName}\" not loaded");
            }

            return configObj;
        }

        public bool ContainsConfig(string configName) => Configs.SingleOrDefault(config => config.Name == configName) is IPropertyContainer;

        public bool StoreConfig(string configName)
        {
            var cfg = Configs.SingleOrDefault(config => config.Name == configName);

            if (cfg is null)
            {
                _viewService.Error($"ConfigManager doesn't contain config with name \"{configName}\"");
            }

            bool success = cfg.Store();

            if (!success)
            {
                _viewService.Error($"Unable store DataContainer \"{configName}\" on \"{cfg.FilePath}\"");
            }


            return success;
        }

        #endregion
    }

    public class ConfigLoadedEvent : PubSubEvent<IPropertyContainer> { }

}
