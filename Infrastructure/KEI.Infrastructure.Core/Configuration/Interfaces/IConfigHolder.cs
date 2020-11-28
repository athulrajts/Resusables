using KEI.Infrastructure.Service;

namespace KEI.Infrastructure
{
    public interface IConfigHolder<T> : IConfigurable
    {
        T Config { get; set; }
        string ConfigName { get; }
        bool StoreConfig();
    }
}
