namespace KEI.Infrastructure.Configuration
{
    public interface IConfigHolder<T>
    {
        T Config { get; set; }
        string ConfigPath { get; }
        string ConfigName { get; }
        bool LoadConfig();
        bool StoreConfig();
    }
}
