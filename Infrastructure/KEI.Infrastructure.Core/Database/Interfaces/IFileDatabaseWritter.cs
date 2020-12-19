using System.Data;
using KEI.Infrastructure.Service;

namespace KEI.Infrastructure.Database
{
    [Service("Database Writter")]
    public interface IFileDatabaseWritter
    {
        public string DestinationPath { get; set; }
        public void Configure(DatabaseSetup schema);
        public void Write(DataRow row);
        public void WriteHeader();
        public void CreateNewFile();
        public bool CanWrite();
    }
}
