using System.Text;
using System.Reflection;
using KEI.Infrastructure.Helpers;
using KEI.Infrastructure.Validation;

namespace KEI.Infrastructure.Database.Models
{
    public class NumericDatabaseColumn : DatabaseColumn
    {
        public Ineqaulity LowerPassFailCriteria { get; set; }
        public Ineqaulity UpperPassFailCriteria { get; set; }
        public double LowerPassFailValue { get; set; }
        public double UpperPassFailValue { get; set; }

        public NumericDatabaseColumn(PropertyInfo propInfo) : base(propInfo) { }

        public NumericDatabaseColumn() { }

        public override bool IsValid(string value)
        {
            if (HasPassFailCriteria == false)
            {
                return true;
            }

            if (value is null || string.IsNullOrEmpty(value))
            {
                return false;
            }

            double doubleValue = double.Parse(value);


            bool retValue = LowerPassFailCriteria switch
            {
                Ineqaulity.GreaterThan => doubleValue > LowerPassFailValue,
                Ineqaulity.GreaterThanOrEqualTo => doubleValue >= LowerPassFailValue,
                _ => true
            };

            if (retValue == false)
            {
                return false;
            }


            retValue = UpperPassFailCriteria switch
            {
                Ineqaulity.LessThan => doubleValue < UpperPassFailValue,
                Ineqaulity.LessThanOrEqualTo => doubleValue <= UpperPassFailValue,
                _ => true
            };

            return retValue;
        }

        public override string HeaderString
        {
            get
            {
                return new StringBuilder()
                    .AppendLine(DisplayName)
                    .AppendLine(string.IsNullOrEmpty(Unit) ? "NA" : Unit)
                    .AppendLine(HasPassFailCriteria ? $"{LowerPassFailCriteria.GetEnumDescription()}{LowerPassFailValue}" : "Always")
                    .Append(HasPassFailCriteria ? $"{UpperPassFailCriteria.GetEnumDescription()}{UpperPassFailValue}" : "Always")
                    .ToString();
            }
        }
    }
}
