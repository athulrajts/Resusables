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
            var retValue = true;

            if (HasPassFailCriteria == false)
                return true;

            if (value is null || string.IsNullOrEmpty(value))
                return true;

            double doubleValue = double.Parse(value);

            switch (LowerPassFailCriteria)
            {
                case Ineqaulity.GreaterThan:
                    retValue = doubleValue > LowerPassFailValue;
                    break;
                case Ineqaulity.GreaterThanOrEqualTo:
                    retValue = doubleValue >= LowerPassFailValue;
                    break;
            }

            if (retValue == false)
                return false;


            switch (UpperPassFailCriteria)
            {
                case Ineqaulity.LessThan:
                    retValue = doubleValue < UpperPassFailValue;
                    break;
                case Ineqaulity.LessThanOrEqualTo:
                    retValue = doubleValue <= UpperPassFailValue;
                    break;
            }

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
