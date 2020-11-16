using KEI.Infrastructure.Service;

namespace KEI.Infrastructure.Configuration
{
    public interface IConfigHolder<T> : IConfigurable
    {
        T Config { get; set; }
        string ConfigName { get; }
        bool StoreConfig();
    }
}
