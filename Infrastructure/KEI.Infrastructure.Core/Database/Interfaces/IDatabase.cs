using System.Collections.Generic;
using System.Data;

namespace KEI.Infrastructure.Database
{
    public interface IDatabase
    {
        public void AddRecord(params IDatabaseContext[] results);
        public void StartSession(DatabaseSetup s);
        public IDatabaseWritter DatabaseWritter { get; }
        public IEnumerable<DatabaseColumn> GetSchema();
        public void CreateNew();
        public DataTable GetData();
    }

    public enum DatabaseCreationMode
    {
        Daily,
        EveryTest
    }
}