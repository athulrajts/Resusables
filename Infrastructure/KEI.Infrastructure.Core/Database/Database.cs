using KEI.Infrastructure.Database.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace KEI.Infrastructure.Database
{
    public class Database : IDatabase
    {
        private DataTable _table;
        private DatabaseSetup _setup;
        private PassFailDatabaseColumn _passFail;
        private IEnumerable<DatabaseColumn> _resultColumns;
        private IEnumerable<AggregateDatabaseColumn> _aggregateColumns;
        public IDatabaseWritter DatabaseWritter { get; private set; }
        
        public Database(IDatabaseWritter dbWriter)
        {
            DatabaseWritter = dbWriter;
        }

        /// <summary>
        /// Initialize database with given config
        /// </summary>
        /// <param name="setup">database setup</param>
        public void StartSession(DatabaseSetup setup)
        {
            _setup = setup;
            _passFail = GetPassFailColumn();
            _resultColumns = GetResultColumns();
            _aggregateColumns = GetAggregateColumns();

            DatabaseWritter.Setup(setup);

            if (NeedToLoadDataTable())
            {
                LoadDataTable();
                DatabaseWritter.CreateNew();
            }
            else if (DatabaseWritter.CanWrite() == false)
            {
                DatabaseWritter.CreateNew();
            }

        }

        private void LoadDataTable()
        {
            _table = new DataTable(_setup.Name);

            foreach (var col in _setup.Schema)
            {
                var newCol = new TaggedDataColumn
                {
                    ColumnName = col.DisplayName,
                    DataType = typeof(object),
                    Caption = col.HeaderString,
                    Tag = col
                };


                _table.Columns.Add(newCol);
            }
        }

        private bool NeedToLoadDataTable()
        {
            /// Loading DB for the first time
            if (_table is null || _table.Columns.Count == 0)
            {
                LoadDataTable();
            }

            // Setup changed
            if (_table.Columns.Count != _setup.Schema.Count())
            {
                return true;
            }

            /// Need to recreate if order of columns changed
            for (int i = 0; i < _setup.Schema.Count(); i++)
            {
                if (_table.Columns[i].ColumnName != _setup.Schema.ElementAt(i).DisplayName)
                {
                    return true;
                }
            }

            /// Need to recreate if setup for any of the columns changed
            for (int i = 0; i < _table.Columns.Count; i++)
            {
                var tableColumn = (_table.Columns[i] as TaggedDataColumn).Tag as DatabaseColumn;
                var schemaColumn = _setup.Schema.ElementAt(i);

                if (schemaColumn.Equals(tableColumn) == false)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Create new Database with Existing config
        /// </summary>
        public void CreateNew()
        {
            if (_setup.CreationMode == DatabaseCreationMode.EveryTest)
            {
                _table.Rows.Clear();
            }

            DatabaseWritter.CreateNew();
        }
        /// <summary>
        /// Get Database information
        /// </summary>
        /// <returns>Database schema</returns>
        public IEnumerable<DatabaseColumn> GetSchema() => _setup.Schema;

        /// <summary>
        /// Append record to database
        /// </summary>
        /// <param name="results">Source</param>
        public void AddRecord(params IDatabaseContext[] results)
        {

            DataRow row = _table.NewRow();

            /// Log all values from who's values can be directly obtained from
            /// <param name="results"></param>
            foreach (var dbColumn in _resultColumns)
            {
                var columnValue = results.TryGetValue(dbColumn);
                row[dbColumn.Name] = columnValue;

                if (dbColumn.IsValid(columnValue?.ToString()) == false)
                {
                    row.SetColumnError(dbColumn.DisplayName, "invalid");
                }
            }

            /// Calculate and update values for <see cref="AggregateDatabaseColumn"/>
            foreach (var aggregateColumn in _aggregateColumns)
            {
                aggregateColumn.UpdateCell(row);
            }

            /// Update pass fail if that column is added to the setup
            _passFail?.UpdateCell(row);

            /// Add completed row to <see cref="DataTable"/> 
            _table.Rows.Add(row);

            /// Write to db.
            DatabaseWritter.Write(row);
        }

        /// <summary>
        /// Get underlying DataTable object representing database
        /// </summary>
        /// <returns></returns>
        public DataTable GetData() => _table;

        private IEnumerable<DatabaseColumn> GetResultColumns()
            => _setup.Schema.Where(x => typeof(AggregateDatabaseColumn).IsAssignableFrom(x.GetType()) == false);
        
        private IEnumerable<AggregateDatabaseColumn> GetAggregateColumns()
            => _setup.Schema.Where(x => x is AggregateDatabaseColumn && x.GetType() != typeof(PassFailDatabaseColumn))
               .Cast<AggregateDatabaseColumn>();

        private PassFailDatabaseColumn GetPassFailColumn() =>
            _setup.Schema.FirstOrDefault(x => x is PassFailDatabaseColumn) as PassFailDatabaseColumn;
    }

    public static class DatabaseExtensions
    {
        public static IDatabaseContext GetContext(this IDatabaseContext[] contexts, string name)
            => contexts.FirstOrDefault(x => x.GetType().Name == name);

        public static object TryGetValue(this IDatabaseContext[] contexts, DatabaseColumn column)
        {
            var context = contexts.GetContext(column.Namespace);

            if (context == null)
            {
                return string.Empty;
            }

            if (context.GetType().GetProperty(column.Name) is PropertyInfo p)
            {
                return p.GetValue(context);
            }

            return string.Empty;
        }
    }
}


