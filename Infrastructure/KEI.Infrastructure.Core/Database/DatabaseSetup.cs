using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using KEI.Infrastructure.Database.Models;

namespace KEI.Infrastructure.Database
{
    /// <summary>
    /// Class to Store the Database Information
    /// </summary>
    public class DatabaseSetup
    {
        #region Constructor
        public DatabaseSetup() { }

        #endregion

        /// <summary>
        /// List of Column Information
        /// </summary>
        public List<DatabaseColumn> Schema { get; set; }

        /// <summary>
        /// File Name of CSV
        /// </summary>
        public string Name { get; set; } = "Database/Test.csv";

        /// <summary>
        /// Get <see cref="DatabaseColumn"/> Objects for given IDatabaseContext instance
        /// </summary>
        /// <param name="t">IDatabaseContext Instance</param>
        /// <returns></returns>
        internal IEnumerable<DatabaseColumn> GetHeadersFor(IDatabaseContext t) => Schema.Where(x => x.Namespace == t.GetType().Name);

        public DatabaseCreationMode CreationMode { get; set; } = DatabaseCreationMode.Daily;

    }

    /// <summary>
    /// Class to store information about columns of database
    /// </summary>
    public class DatabaseSchema : IEnumerable<DatabaseColumn>
    {
        public List<DatabaseColumn> Columns;

        public DatabaseSchema()
        {
            Columns = new List<DatabaseColumn>();
        }

        /// <summary>
        /// Create Schema for list of <see cref="DatabaseColumn"/>
        /// </summary>
        /// <param name="columns"></param>
        public DatabaseSchema(IEnumerable<DatabaseColumn> columns)
        {
            Columns = new List<DatabaseColumn>();

            foreach (var col in columns)
            {
                Columns.Add(col);
            }
        }

        /// <summary>
        /// Add <see cref="DatabaseColumn"/> to Schema
        /// </summary>
        /// <param name="column"></param>
        public void AddColumn(DatabaseColumn column)
        {
            Columns.Add(column);
        }

        #region IEnumerable<T> Members
        public IEnumerator<DatabaseColumn> GetEnumerator() => Columns.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Columns.GetEnumerator();

        #endregion

        /// <summary>
        /// Create DatabaseSchema for type of IDataContext implementations
        /// </summary>
        /// <typeparam name="T">IDataContext Type</typeparam>
        /// <returns>Database Schema</returns>
        public static DatabaseSchema SchemaFor<T>() where T : IDatabaseContext
        {
            var schema = new DatabaseSchema();

            var props = typeof(T).GetProperties();

            foreach (var prop in props)
            {
                if (prop.PropertyType == typeof(int) ||
                   prop.PropertyType == typeof(float) ||
                   prop.PropertyType == typeof(double))
                {
                    schema.AddColumn(new NumericDatabaseColumn(prop));
                }
                else
                {
                    schema.AddColumn(new DatabaseColumn(prop));
                }
            }

            return schema;
        }

        /// <summary>
        /// Create Database schema for list of IDataContext Types
        /// </summary>
        /// <param name="types">List of IDataContext types</param>
        /// <returns>Database Schema</returns>
        public static DatabaseSchema SchemaFor(params Type[] types)
        {
            var schema = new DatabaseSchema();

            foreach (var type in types)
            {
                if (!typeof(IDatabaseContext).IsAssignableFrom(type))
                    continue;

                var props = type.GetProperties();

                foreach (var prop in props)
                {
                    schema.AddColumn(new DatabaseColumn(prop));
                } 
            }

            return schema;
        }

    }
}
