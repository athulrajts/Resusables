using KEI.Infrastructure.Database;
using System.Data;

namespace Application.Core.Interfaces
{
    public interface IDatabaseManager
    {
        IDatabase GetDatabase(string databaseName);
        void InitializeDatabase(string databaseName);
        IDatabase this[string databaseName] { get; }
        DataTable GetData(string dbPath);
    }
}
