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
        public List<DatabaseColumn> Columns { get; set; }

        /// <summary>
        /// File Name of CSV
        /// </summary>
        public string Name { get; set; } = "Database/Test.csv";

        /// <summary>
        /// Get <see cref="DatabaseColumn"/> Objects for given IDatabaseContext instance
        /// </summary>
        /// <param name="t">IDatabaseContext Instance</param>
        /// <returns></returns>
        internal IEnumerable<DatabaseColumn> GetHeadersFor(IDatabaseContext t) => Columns.Where(x => x.Namespace == t.GetType().Name);

        public DatabaseCreationMode CreationMode { get; set; } = DatabaseCreationMode.Daily;

    }

    /// <summary>
    /// Class to store information about columns of database
    /// </summary>
    public static class DatabaseColumnGenerator
    {
        /// <summary>
        /// Create DatabaseSchema for type of IDataContext implementations
        /// </summary>
        /// <typeparam name="T">IDataContext Type</typeparam>
        /// <returns>Database Schema</returns>
        public static List<DatabaseColumn> ColumnsFor<T>() where T : IDatabaseContext
        {
            var schema = new List<DatabaseColumn>();

            var props = typeof(T).GetProperties();

            foreach (var prop in props)
            {
                if (prop.PropertyType == typeof(int) ||
                   prop.PropertyType == typeof(float) ||
                   prop.PropertyType == typeof(double))
                {
                    schema.Add(new NumericDatabaseColumn(prop));
                }
                else
                {
                    schema.Add(new DatabaseColumn(prop));
                }
            }

            return schema;
        }

        /// <summary>
        /// Create Database schema for list of IDataContext Types
        /// </summary>
        /// <param name="types">List of IDataContext types</param>
        /// <returns>Database Schema</returns>
        public static List<DatabaseColumn> ColumnsFor(params Type[] types)
        {
            var schema = new List<DatabaseColumn>();

            foreach (var type in types)
            {
                if (!typeof(IDatabaseContext).IsAssignableFrom(type))
                    continue;

                var props = type.GetProperties();

                foreach (var prop in props)
                {
                    if (prop.PropertyType == typeof(int) ||
                       prop.PropertyType == typeof(float) ||
                       prop.PropertyType == typeof(double))
                    {
                        schema.Add(new NumericDatabaseColumn(prop));
                    }
                    else
                    {
                        schema.Add(new DatabaseColumn(prop));
                    }
                } 
            }

            return schema;
        }

    }
}
