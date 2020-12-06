using System.IO;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using KEI.Infrastructure.Validation;
using KEI.Infrastructure.Database.Models;

namespace KEI.Infrastructure.Database
{
    public class CSVDatabaseReader : IDatabaseReader
    {
        private static readonly Regex criteriaRegex = new Regex(@"(<|>|>=|<=)(-?\d+(\.)?(\d+)?)");
        private DataTable _table;
        
        public DataTable ReadDatabase(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }

            var lines = File.ReadAllLines(filePath);

            // There will be atleast 5 lines of header, if not we have wrong file
            if (lines.Length < 5)
            {
                return null;
            }

            ReadHeader(lines.Take(5));

            ReadDatabase(lines.Skip(5));

            return _table;
        }

        private void ReadDatabase(IEnumerable<string> dataRows)
        {
            foreach (var row in dataRows)
            {
                _table.Rows.Add(row.Split(','));
            }
        }

        private void ReadHeader(IEnumerable<string> metadata)
        {
            _table = new DataTable();

            // Line 0 : Header
            // Line 1 : Unit
            // Line 2 : $(LowerPassFailCritria)$(LowerPassFailValue)
            // Line 3 : $(UpperPassFailCritria)$(UpperPassFailValue)
            // Line 4 : Blank

            var headers = metadata.ElementAt(0).Split(',');
            var units = metadata.ElementAt(1).Split(',');
            var lowerValues = metadata.ElementAt(2).Split(',');
            var upperValues = metadata.ElementAt(3).Split(',');

            for (int i = 0; i < headers.Length; i++)
            {

                DatabaseColumn dbColumn = CreateDBColumn(headers[i], units[i], lowerValues[i], upperValues[i]);

                var newCol = new TaggedDataColumn
                {
                    ColumnName = dbColumn.DisplayName,
                    DataType = typeof(object),
                    Caption = dbColumn.HeaderString,
                    Tag = dbColumn
                };

                _table.Columns.Add(newCol);

            }
        }

        private DatabaseColumn CreateDBColumn(string header, string unit, string lower, string upper)
        {

            if ((lower == "Always" || string.IsNullOrEmpty(lower)) &&
               (upper == "Always" || string.IsNullOrEmpty(upper)))
            {
                return new DatabaseColumn
                {
                    DisplayName = header,
                    HasPassFailCriteria = false,
                    Name = header,
                    Unit = unit,
                };
            }

            var lowerMatch = criteriaRegex.Match(lower);
            var upperMatch = criteriaRegex.Match(upper);

            return new NumericDatabaseColumn
            {
                DisplayName = header,
                HasPassFailCriteria = true,
                Name = header,
                Unit = unit,
                LowerPassFailCriteria = GetInequalityFromString(lowerMatch.Groups[1].Value),
                LowerPassFailValue = double.Parse(lowerMatch.Groups[2].Value),
                UpperPassFailCriteria = GetInequalityFromString(upperMatch.Groups[1].Value),
                UpperPassFailValue = double.Parse(upperMatch.Groups[2].Value)
            };
        
        }

        private static Ineqaulity GetInequalityFromString(string symbol)
        {
            return symbol switch
            {
                "<" => Ineqaulity.LessThan,
                ">" => Ineqaulity.GreaterThan,
                "<=" => Ineqaulity.LessThanOrEqualTo,
                ">=" => Ineqaulity.GreaterThanOrEqualTo,
                "!=" => Ineqaulity.NotEqualTo,
                _ => Ineqaulity.LessThan,
            };
        }

    }
}
