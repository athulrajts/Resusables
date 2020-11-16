using KEI.Infrastructure.Service;
using System.Data;

namespace KEI.Infrastructure.Database
{
    [Service("Database Writter")]
    public interface IDatabaseWritter
    {
        string DestinationPath { get; set; }
        void Setup(DatabaseSetup schema);
        void Write(DataRow row);
        void WriteHeader();
        void CreateNew();
        bool CanWrite();
    }
}
