using System.Data;
using System.Collections.Generic;
using Prism.Events;
using KEI.Infrastructure.Database;
using KEI.Infrastructure.Configuration;
using Application.Core.Interfaces;
using KEI.Infrastructure.Events;

namespace Application.Core.Modules
{
    public class DatabaseManager : IDatabaseManager
    {
        private readonly Dictionary<string, IDatabase> _databaseCollection;
        private readonly IDatabaseReader _databaseReader;
        private IPropertyContainer _currentRecipe;

        public DatabaseManager(IEventAggregator eventAggregator, IDatabaseWritter databaseWritter, IDatabaseReader databaseReader)
        {
            _databaseReader = databaseReader;

            _databaseCollection = new Dictionary<string, IDatabase>();
            _databaseCollection.Add($"{ApplicationMode.Production} DB", new Database(databaseWritter));
            _databaseCollection.Add($"{ApplicationMode.Engineering} DB", new Database(databaseWritter));

            eventAggregator.GetEvent<RecipeLoadedEvent>().Subscribe(rcp => _currentRecipe = rcp);
        }

        public IDatabase this[string databaseName] => _databaseCollection.ContainsKey(databaseName) ? _databaseCollection[databaseName] : null;

        public DataTable GetData(string dbPath) => _databaseReader.ReadDatabase(dbPath);

        public IDatabase GetDatabase(string databaseName) => this[databaseName];

        public void InitializeDatabase(string databaseName)
        {
            IPropertyContainer setup = null;
            _currentRecipe.Get(databaseName, ref setup);

            if(setup.Morph() is DatabaseSetup dbs)
            {
                GetDatabase(databaseName).StartSession(dbs);
            }

        }
    }
}
