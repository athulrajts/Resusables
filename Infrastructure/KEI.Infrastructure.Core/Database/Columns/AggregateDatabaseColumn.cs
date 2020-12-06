using System.Data;

namespace KEI.Infrastructure.Database.Models
{
    public abstract class AggregateDatabaseColumn : DatabaseColumn
    {
        public AggregateDatabaseColumn()
        {
            Namespace = "Aggregate";
        }

        public abstract void UpdateCell(DataRow dataRow);
    }
}
