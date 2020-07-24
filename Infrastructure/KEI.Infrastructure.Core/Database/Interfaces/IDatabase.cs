using System.Collections.Generic;
using System.Data;

namespace KEI.Infrastructure.Database
{
    public interface IDatabase
    {
        void AddRecord(params IDatabaseContext[] results);
        void StartSession(DatabaseSetup s);
        IDatabaseWritter DatabaseWritter { get; }
        IEnumerable<DatabaseColumn> GetSchema();
        void CreateNew();
        DataTable GetData();
    }

    public enum DatabaseCreationMode
    {
        Daily,
        EveryTest
    }
}