using System.Data;

namespace KEI.Infrastructure.Database
{
    public interface IFileDatabase
    {
        public void AddRecord(params IDatabaseContext[] results);
        public void StartSession(DatabaseSetup s);
        public void CreateNew();
        public DataTable GetData();
        public string GetFilePath();
    }

    public enum DatabaseCreationMode
    {
        Daily,
        EveryTest
    }
}