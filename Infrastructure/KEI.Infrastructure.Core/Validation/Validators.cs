using System;
using System.Reflection;
using KEI.Infrastructure.Validation.Attributes;

namespace KEI.Infrastructure.Validation
{
    public static class Validators
    {
        public static ValidationRule GetTypeValidator(this IConvertible obj) => Type(obj.GetType());

        public static ValidationRule Type(Type t) => new TypeValidator { Type = t };

        public static bool IsValid(object obj)
        {
            var props = obj.GetType().GetProperties();

            foreach (var prop in props)
            {
                var validators = prop.GetCustomAttributes<ValidationAttribute>();

                foreach (var validator in validators)
                {
                    var result = validator.Validate(prop.GetValue(obj));

                    if (!result.IsValid)
                        return false;
                }
            }

            return true;
        }

        #region Length (string)
        public static ValidationRule Length(int minLength, int maxLength) => new LengthValidator { Min = minLength, Max = maxLength };
        public static ValidationRule Length(int exactLength) => new LengthValidator { Min = exactLength, Max = exactLength };
        public static ValidationRule MaxLength(int maxLength) => new LengthValidator { Min = 0, Max = maxLength };
        public static ValidationRule MinLength(int minLength) => new LengthValidator { Min = minLength, Max = int.MaxValue };
       
        #endregion
        
        #region String
        public static ValidationRule NotNullOrEmpty() => new NotNullOrEmptyValidator();
        public static ValidationRule File() => new PathValidator { Mode = PathValidationMode.File };
        public static ValidationRule Directory() => new PathValidator { Mode = PathValidationMode.Directory };
        
        #endregion
        
        #region Numeric
        public static ValidationRule Range(double minValue, double maxValue) => new RangeValidator { MinValue = minValue, MaxValue = maxValue };
        public static ValidationRule Max(double maxValue) => new RangeValidator { MinValue = double.MinValue, MaxValue = maxValue };
        public static ValidationRule Min(double minValue) => new RangeValidator { MinValue = minValue, MaxValue = double.MaxValue };
        public static ValidationRule OutsideRange(double minValue, double maxValue) => new RangeValidator { MinValue = minValue, MaxValue = maxValue, Invert = true };
        public static ValidationRule Positive() => new NumberSignValidator { IsPositive = true };
        public static ValidationRule Negative() => new NumberSignValidator { IsPositive = false };
       
        #endregion
    }
}
