using System.Collections.Generic;

namespace KEI.Infrastructure.Validation
{
    public class MustBeOneOfValidator : ValidationRule
    {
        public List<string> AllowedValues { get; set; } = new List<string>();
        
        public override ValidationResult Validate(object value)
        {
            if (value == null)
            {
                return ValidationFailed("Value cannot be empty");
            }

            if (AllowedValues.Contains(value.ToString()) == false)
            {
                return ValidationFailed($"{value} is not one of {string.Join("|", AllowedValues)}");
            }

            return ValidationSucces();
        }
    }
}
