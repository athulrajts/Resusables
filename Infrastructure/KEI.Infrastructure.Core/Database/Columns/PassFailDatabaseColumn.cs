using System.Data;

namespace KEI.Infrastructure.Database.Models
{
    public class PassFailDatabaseColumn : AggregateDatabaseColumn
    {
        public string PassString { get; set; } = "PASS";
        public string FailString { get; set; } = "FAIL";

        public PassFailDatabaseColumn()
        {
            Name = DisplayName = "Result";
            TypeString = typeof(bool).FullName;
        }
        public override void UpdateCell(DataRow dataRow)
            => dataRow[DisplayName] = dataRow.HasErrors ? FailString : PassString;

        public override string HeaderString => DisplayName;

        public override string GetUnit() => string.Empty;
    }
}
