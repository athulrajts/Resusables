using System.Data;
using System.Collections.Generic;
using Prism.Events;
using KEI.Infrastructure;
using KEI.Infrastructure.Events;
using KEI.Infrastructure.Database;
using Application.Core.Interfaces;

namespace Application.Core.Modules
{
    public class DatabaseManager : IDatabaseManager
    {
        private readonly Dictionary<string, IFileDatabase> _databaseCollection;
        private readonly IFileDatabaseReader _databaseReader;
        private IPropertyContainer _currentRecipe;

        public DatabaseManager(IEventAggregator eventAggregator, IFileDatabaseWritter databaseWritter, IFileDatabaseReader databaseReader)
        {
            _databaseReader = databaseReader;

            _databaseCollection = new Dictionary<string, IFileDatabase>();
            _databaseCollection.Add($"{ApplicationMode.Production}_DB", new Database(databaseWritter));
            _databaseCollection.Add($"{ApplicationMode.Engineering}_DB", new Database(databaseWritter));

            eventAggregator.GetEvent<RecipeLoadedEvent>().Subscribe(rcp => _currentRecipe = rcp);
        }

        public IFileDatabase this[string databaseName] => _databaseCollection.ContainsKey(databaseName) ? _databaseCollection[databaseName] : null;

        public DataTable GetData(string dbPath) => _databaseReader.ReadDatabase(dbPath);

        public IFileDatabase GetDatabase(string databaseName) => this[databaseName];

        public void InitializeDatabase(string databaseName)
        {
            IPropertyContainer setup = null;
            _currentRecipe.GetValue(databaseName, ref setup);

            if(setup.Morph() is DatabaseSetup dbs)
            {
                GetDatabase(databaseName).StartSession(dbs);
            }

        }
    }
}
