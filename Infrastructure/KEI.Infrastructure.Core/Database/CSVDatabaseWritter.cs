using System;
using System.IO;
using System.Linq;
using System.Data;
using KEI.Infrastructure.Helpers;
using KEI.Infrastructure.Database.Models;

namespace KEI.Infrastructure.Database
{
    /// <summary>
    /// Writes Database into a CSV file
    /// </summary>
    public class CSVDatabaseWritter : IDatabaseWritter
    {
        /// <summary>
        /// Path to save file
        /// </summary>
        public string DestinationPath { get; set; }


        private DatabaseSetup _setup;
        public CSVDatabaseWritter(DatabaseSetup setup)
        {
            _setup = setup;
        }

        public CSVDatabaseWritter()
        {

        }

        /// <summary>
        /// Configure the DB
        /// </summary>
        /// <param name="schema"></param>
        public void Setup(DatabaseSetup setup) => _setup = setup;

        /// <summary>
        /// Append Row to CSV
        /// </summary>
        /// <param name="row">DataRow</param>
        public void Write(DataRow row)
        {
            var str = string.Join(",", row.ItemArray.Select(x => x));
            using (var sw = new StreamWriter(DestinationPath, append: true) { AutoFlush = true })
            {
                sw.WriteLine(str);
            }
        }

        /// <summary>
        /// Create a new .csv file with existing config
        /// </summary>
        public void CreateNew() => WriteHeader();

        public bool CanWrite()
        {
            var fileName = Path.GetFileNameWithoutExtension(_setup.Name);
            var directory = Path.GetDirectoryName(_setup.Name);
            var now = DateTime.Now;

            DestinationPath = _setup.CreationMode == DatabaseCreationMode.EveryTest
                ? Path.Combine(directory, $"{fileName}_[{now.ToString("dd-MMM-yyyy")}]_[{now.ToString("HH_mm_ss")}].csv")
                : Path.Combine(directory, $"{fileName}_[{now.ToString("dd-MMM-yyyy")}].csv");

            return File.Exists(DestinationPath);
        }

        /// <summary>
        /// Writes header of the database
        /// </summary>
        public void WriteHeader()
        {
            var fileName = Path.GetFileNameWithoutExtension(_setup.Name);
            var directory = Path.GetDirectoryName(_setup.Name);
            var now = DateTime.Now;

            DestinationPath = _setup.CreationMode == DatabaseCreationMode.EveryTest
                ? Path.Combine(directory, $"{fileName}_[{now.ToString("dd-MMM-yyyy")}]_[{now.ToString("HH_mm_ss")}].csv")
                : Path.Combine(directory, $"{fileName}_[{now.ToString("dd-MMM-yyyy")}].csv");

            var schema = _setup.Schema;
            string headers = string.Join(",", schema.Select(x => x.DisplayName));
            string units = string.Join(",", schema.Select(x => x.GetUnit()));
            string lower = string.Empty;
            string upper = string.Empty;

            foreach (var header in schema)
            {
                if (header is NumericDatabaseColumn ndc)
                {
                    lower += header.HasPassFailCriteria ? $"{ndc.LowerPassFailCriteria.GetEnumDescription()}{ndc.LowerPassFailValue}" : "Always";
                    upper += header.HasPassFailCriteria ? $"{ndc.UpperPassFailCriteria.GetEnumDescription()}{ndc.UpperPassFailValue}" : "Always"; 
                }

                if (header != schema.LastOrDefault())
                {
                    lower += ",";
                    upper += ",";
                }
            }

            string empty = string.Join(",", schema.Select(x => ""));

            FileHelper.CreateDirectoryIfNotExist(DestinationPath);

            using (var sw = new StreamWriter(DestinationPath) { AutoFlush = true })
            {
                sw.WriteLine(headers);
                sw.WriteLine(units);
                sw.WriteLine(lower);
                sw.WriteLine(upper);
                sw.WriteLine(empty);
            }
        }
    }
}
