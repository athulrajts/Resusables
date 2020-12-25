using System.ComponentModel;
using System.Xml.Serialization;
using KEI.Infrastructure.Helpers;

namespace KEI.Infrastructure.Validation
{
    public enum Ineqaulity
    {
        [Description("<")]
        LessThan,

        [Description("<=")]
        LessThanOrEqualTo,

        [Description(">")]
        GreaterThan,

        [Description(">=")]
        GreaterThanOrEqualTo,

        [Description("!=")]
        NotEqualTo
    }

    public class LinearInequalityValidator : ValidationRule
    {
        private Ineqaulity ineqaulity;
        private double limit;

        [XmlAttribute]
        public Ineqaulity Ineqaulity
        {
            get { return ineqaulity; }
            set { SetValidationProperty(ref ineqaulity, value); }
        }

        [XmlAttribute]
        public double Limit
        {
            get { return limit; }
            set { SetValidationProperty(ref limit, value); }
        }

        public override ValidationResult Validate(object value)
        {
            if (value == null)
                return CannotBeNull();

            string str = value.ToString();

            if (!double.TryParse(str, out double dVal))
            {
                return ValidationFailed($"{str} cannot be parsed to {typeof(double).FullName}");
            }

            switch (Ineqaulity)
            {
                case Ineqaulity.LessThan:
                    if (dVal >= Limit)
                    {
                        return ValidationFailed($"{dVal} cannot be greater than or equal to {Limit}");
                    }
                    break;
                case Ineqaulity.LessThanOrEqualTo:
                    if (dVal > Limit)
                    {
                        return ValidationFailed($"{dVal} cannot be greater than {Limit}");
                    }
                    break;
                case Ineqaulity.GreaterThan:
                    if (dVal <= Limit)
                    {
                        return ValidationFailed($"{dVal} cannot be less than or equal to {Limit}");
                    }
                    break;
                case Ineqaulity.GreaterThanOrEqualTo:
                    if (dVal < Limit)
                    {
                        return ValidationFailed($"{dVal} cannot be less than {Limit}");
                    }
                    break;
                case Ineqaulity.NotEqualTo:
                    if (dVal != Limit)
                    {
                        return ValidationFailed($"{dVal} should be {Limit}");
                    }
                    break;
            }

            return ValidationSucces();
        }

        public override string StringRepresentation => $"{Ineqaulity.GetEnumDescription()} {Limit}";
    }
}
