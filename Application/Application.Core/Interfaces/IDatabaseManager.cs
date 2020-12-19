using KEI.Infrastructure.Database;
using System.Data;

namespace Application.Core.Interfaces
{
    public interface IDatabaseManager
    {
        IFileDatabase GetDatabase(string databaseName);
        void InitializeDatabase(string databaseName);
        IFileDatabase this[string databaseName] { get; }
        DataTable GetData(string dbPath);
    }
}
