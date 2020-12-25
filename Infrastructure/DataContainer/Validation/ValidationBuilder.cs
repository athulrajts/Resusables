using System;

namespace KEI.Infrastructure.Validation
{
    public class ValidationBuilder
    {
        public ValidatorGroup Validator { get; set; } = new ValidatorGroup();

        public ValidationBuilder(ValidatorGroup validationGroup)
        {
            if (validationGroup != null)
            {
                Validator = validationGroup; 
            }
        }

        public ValidationBuilder() { }

        public static ValidationBuilder Create(bool cascade = false) => new ValidationBuilder() { Validator = new ValidatorGroup(cascade) };

        private ValidationBuilder AddRule(ValidationRule validator)
        {
            Validator?.Rules.Add(validator);
            return this;
        }
        public ValidationBuilder Custom(ValidationRule customValidator) => AddRule(customValidator);

        public ValidationBuilder Type(Type t) => AddRule(new TypeValidator { Type = t });


        #region Length (string)
        public ValidationBuilder Length(int minLength, int maxLength) => AddRule(new LengthValidator { Min = minLength, Max = maxLength });

        public ValidationBuilder Length(int exactLength) => AddRule(new LengthValidator { Min = exactLength, Max = exactLength });

        public ValidationBuilder MaxLength(int maxLength) => AddRule(new LengthValidator { Min = int.MinValue, Max = maxLength });

        public ValidationBuilder MinLength(int minLength) => AddRule(new LengthValidator { Min = minLength, Max = int.MaxValue });

        #endregion

        #region String
        public ValidationBuilder NotNullOrEmpty() => AddRule(new NotNullOrEmptyValidator());

        public ValidationBuilder File() => AddRule(new PathValidator { Mode = PathValidationMode.File });

        public ValidationBuilder Directory() => AddRule(new PathValidator { Mode = PathValidationMode.Directory });

        #endregion

        #region Numeric
        public ValidationBuilder Range(double minValue, double maxValue) => AddRule(new RangeValidator { MinValue = minValue, MaxValue = maxValue });

        public ValidationBuilder Range(Func<double> minValueGetter, Func<double> maxValueGetter) => AddRule(new RangeValidator(minValueGetter, maxValueGetter));

        public ValidationBuilder Max(double maxValue) => AddRule(new RangeValidator { MinValue = double.MinValue, MaxValue = maxValue });

        public ValidationBuilder Min(double minValue) => AddRule(new RangeValidator { MinValue = minValue, MaxValue = double.MaxValue });

        public ValidationBuilder OutsideRange(double minValue, double maxValue) => AddRule(new RangeValidator { MinValue = minValue, MaxValue = maxValue, Invert = true });

        public ValidationBuilder Positive() => AddRule(new NumberSignValidator { IsPositive = true });

        public ValidationBuilder Negative() => AddRule(new NumberSignValidator { IsPositive = false });

        #endregion

    }
}
