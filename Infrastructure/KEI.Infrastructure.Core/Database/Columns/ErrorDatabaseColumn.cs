using System.Data;
using System.Linq;

namespace KEI.Infrastructure.Database.Models
{
    public class ErrorDatabaseColumn : AggregateDatabaseColumn
    {
        public ErrorDatabaseColumn()
        {
            Name = DisplayName = "Errors";
            TypeString = typeof(string).FullName;
        }
        public override void UpdateCell(DataRow dataRow)
            => dataRow[DisplayName] = string.Join(",", dataRow.GetColumnsInError().Select(x => x.ColumnName));

        public override string HeaderString => DisplayName;
        public override string GetUnit() => string.Empty;
    }
}
