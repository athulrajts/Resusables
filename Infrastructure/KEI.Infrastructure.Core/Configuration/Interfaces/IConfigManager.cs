using System.Collections.ObjectModel;

namespace KEI.Infrastructure.Configuration
{
    public interface IConfigManager
    {
        ObservableCollection<IPropertyContainer> Configs { get; set; }

        IPropertyContainer GetConfig(string configName);

        bool ContainsConfig(string configName);

        bool StoreConfig(string configName);
    }
}
