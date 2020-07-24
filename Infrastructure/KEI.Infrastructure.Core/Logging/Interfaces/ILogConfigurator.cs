namespace KEI.Infrastructure
{
    public interface ILogConfigurator
    {
        int NumOfLogFiles { get; set; }
        int LogFileSize { get; set; }
        bool IncludeDebugOutput { get; set; }
        string LogFilename { get; set; }

        void ReadConfiguration();
        void StoreConfiguration();
    }
}
