using System;
using System.Collections.Generic;

namespace KEI.Infrastructure.Validation.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Property, Inherited = true)]
    public class ValidationAttribute : Attribute
    {
        protected ValidationRule validator { get; set; }
        public ValidationResult Validate(object obj) => validator.Validate(obj);
        public ValidationRule GetValidator() => validator;
    }

    [AttributeUsage(validOn: AttributeTargets.Property)]
    public class Length : ValidationAttribute
    {
        public Length(int exactLength)
        {
            validator = new LengthValidator() { Max = exactLength, Min = exactLength };
        }

        public Length(int minLength, int maxLength)
        {
            validator = new LengthValidator() { Max = maxLength, Min = minLength };
        }
    }

    [AttributeUsage(validOn: AttributeTargets.Property)]
    public class Positive : ValidationAttribute
    {
        public Positive()
        {
            validator = new NumberSignValidator { IsPositive = true};
        }
    }

    [AttributeUsage(validOn: AttributeTargets.Property)]
    public class Negative : ValidationAttribute
    {
        public Negative()
        {
            validator = new NumberSignValidator { IsPositive = false };
        }
    }

    [AttributeUsage(validOn: AttributeTargets.Property)]
    public class MustBeOneOf : ValidationAttribute
    {
        public MustBeOneOf(string values, char delimiter = ',')
        {
            validator = new MustBeOneOfValidator { AllowedValues = new List<string>(values.Split(delimiter)) };
        }
    }

    [AttributeUsage(validOn: AttributeTargets.Property)]
    public class NotEmpty : ValidationAttribute
    {
        public NotEmpty()
        {
            validator = new NotNullOrEmptyValidator();
        }
    }

    [AttributeUsage(validOn: AttributeTargets.Property)]
    public class InclusiveRange : ValidationAttribute
    {
        public InclusiveRange(double minValue, double maxValue)
        {
            validator = new RangeValidator { MinValue = minValue, MaxValue = maxValue };
        }
    }


    [AttributeUsage(validOn: AttributeTargets.Property)]
    public class ExclusiveRange : ValidationAttribute
    {
        public ExclusiveRange(double minValue, double maxValue)
        {
            validator = new RangeValidator { MinValue = minValue, MaxValue = maxValue, ExcludeMaxValue = true, ExcludeMinValue = true };
        }
    }

    [AttributeUsage(validOn: AttributeTargets.Property)]
    public class FileAttribute : ValidationAttribute
    {
        public FileAttribute()
        {
            validator = new PathValidator { Mode = PathValidationMode.File };
        }
    }

    [AttributeUsage(validOn: AttributeTargets.Property)]
    public class FolderAttribute : ValidationAttribute
    {
        public FolderAttribute()
        {
            validator = new PathValidator { Mode = PathValidationMode.Directory };
        }
    }

    [AttributeUsage(validOn: AttributeTargets.Property)]
    public class LessThan : ValidationAttribute
    {
        public LessThan(double value)
        {
            validator = new LinearInequalityValidator { Ineqaulity = Ineqaulity.LessThan, Limit = value };
        }
    }

    [AttributeUsage(validOn: AttributeTargets.Property)]
    public class LessThanOrEqual : ValidationAttribute
    {
        public LessThanOrEqual(double value)
        {
            validator = new LinearInequalityValidator { Ineqaulity = Ineqaulity.LessThanOrEqualTo, Limit = value };
        }
    }

    [AttributeUsage(validOn: AttributeTargets.Property)]
    public class GreaterThan : ValidationAttribute
    {
        public GreaterThan(double value)
        {
            validator = new LinearInequalityValidator { Ineqaulity = Ineqaulity.GreaterThan, Limit = value };
        }
    }

    [AttributeUsage(validOn: AttributeTargets.Property)]
    public class GreaterThanOrEqual : ValidationAttribute
    {
        public GreaterThanOrEqual(double value)
        {
            validator = new LinearInequalityValidator { Ineqaulity = Ineqaulity.GreaterThanOrEqualTo, Limit = value };
        }
    }

}
