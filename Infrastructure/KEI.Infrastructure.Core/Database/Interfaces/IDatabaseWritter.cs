using System.Data;
using KEI.Infrastructure.Service;

namespace KEI.Infrastructure.Database
{
    [Service("Database Writter")]
    public interface IDatabaseWritter
    {
        public string DestinationPath { get; set; }
        public void Setup(DatabaseSetup schema);
        public void Write(DataRow row);
        public void WriteHeader();
        public void CreateNew();
        public bool CanWrite();
    }
}
