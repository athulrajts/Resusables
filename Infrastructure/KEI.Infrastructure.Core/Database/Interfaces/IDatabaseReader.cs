using KEI.Infrastructure.Service;
using System.Data;

namespace KEI.Infrastructure.Database
{
    [Service("Database Reader", typeof(CSVDatabaseReader))]
    public interface IDatabaseReader
    {
        DataTable ReadDatabase(string filePath);
    }
}
