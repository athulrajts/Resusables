using System.Data;

namespace KEI.Infrastructure.Database.Models
{
    public class ExpressionDatabaseColumn : DatabaseColumn
    {
        public string Expression { get; set; }

        public override DataColumn GetDataTableColumn()
        {
            return new TaggedDataColumn
            {
                ColumnName = DisplayName,
                Tag = this,
                DataType = typeof(object),
                Caption = HeaderString,
                Expression = Expression
            };
        }
    }
}
