using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace KEI.Infrastructure.Validation
{
    public class RangeValidator : ValidationRule
    {
        public RangeValidator(Func<double> minValGetter, Func<double> maxValGetter)
        {
            minValueGetter = minValGetter;
            maxValueGetter = maxValGetter;
        }
        public RangeValidator() { }

        private double maxValue = double.PositiveInfinity;
        private double minValue = double.NegativeInfinity;
        private bool excludeMaxValue;
        private bool excludeMinValue;
        private bool invert;
        private Func<double> maxValueGetter;
        private Func<double> minValueGetter;

        [XmlAttribute]
        public double MaxValue
        {
            get { return maxValue; }
            set { SetValidationProperty(ref maxValue, value); }
        }
        [XmlAttribute]
        public double MinValue
        {
            get { return minValue; }
            set { SetValidationProperty(ref minValue, value); }
        }
        [XmlAttribute]
        public bool ExcludeMaxValue
        {
            get { return excludeMaxValue; }
            set { SetValidationProperty(ref excludeMaxValue, value); }
        }
        [XmlAttribute]
        public bool ExcludeMinValue
        {
            get { return excludeMinValue; }
            set { SetValidationProperty(ref excludeMinValue, value); }
        }
        [XmlAttribute]
        public bool Invert
        {
            get { return invert; }
            set { SetValidationProperty(ref invert, value); }
        }

        public override ValidationResult Validate(object value)
        {
            if (maxValueGetter != null)
                MaxValue = maxValueGetter();

            if (minValueGetter != null)
                MinValue = minValueGetter();

            string str = value.ToString();
            double upperLimit = ExcludeMaxValue ? MaxValue - 1 : MaxValue;
            double lowerLimit = ExcludeMinValue ? MinValue + 1 : MinValue;
            string errMsg = $"Value should be in the range ({lowerLimit},{upperLimit})";
            string invErrMsg = $"Value should not be in the range ({lowerLimit},{upperLimit})";

            double temp;
            if (double.IsPositiveInfinity(MaxValue) == false)
            {
                if (double.TryParse(str, out temp))
                {
                    if (Invert)
                    {
                        if (temp <= upperLimit && temp >= lowerLimit)
                            return ValidationFailed(invErrMsg);
                    }
                    else
                    {
                        if (temp > upperLimit)
                            return ValidationFailed(errMsg);
                    }
                }
                else
                {
                    return ValidationFailed($"Value is not convertable to {typeof(double)}");
                }
            }

            if (double.IsNegativeInfinity(MinValue) == false)
            {
                if (double.TryParse(str, out temp))
                {
                    if (Invert)
                    {
                        if (temp >= lowerLimit && temp <= upperLimit)
                            return ValidationFailed(invErrMsg);
                    }
                    else
                    {
                        if (temp < lowerLimit)
                            return ValidationFailed(errMsg);
                    }
                }
                else
                {
                    return ValidationFailed($"Value is not convertable to {typeof(double)}");
                }
            }

            return ValidationSucces();
        }

        public override string StringRepresentation
        {
            get
            {
                string openingBrackets = ExcludeMinValue ? "(" : "[";
                string closingBrackets = ExcludeMaxValue ? ")" : "]";
                string range = Invert ? "Outside" : "Inside";

                return $"{range}{openingBrackets}{MinValue} - {MaxValue}{closingBrackets}";
            }
        }
    }

    public class NumberSignValidator : ValidationRule
    {
        private bool isPositive;

        [XmlAttribute]
        public bool IsPositive
        {
            get { return isPositive; }
            set { SetValidationProperty(ref isPositive, value); }
        }

        [Browsable(false)]
        public override string StringRepresentation => IsPositive ? "+VE" : "-VE";

        public override ValidationResult Validate(object value)
        {
            if (value == null)
                return CannotBeNull();

            double num = 0;

            if (!double.TryParse(value.ToString(), out num))
                return ValidationFailed($"{value} is not convertable to {typeof(double).FullName}");

            if (IsPositive)
            {
                if (num < 0)
                    return ValidationFailed($"{value} is not a positive number");
            }
            else
            {
                if (num > 0)
                    return ValidationFailed($"{value} is not a negative number");
            }

            return ValidationSucces();
        }

    }
}
