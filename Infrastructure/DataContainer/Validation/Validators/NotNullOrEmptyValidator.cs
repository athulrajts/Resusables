namespace KEI.Infrastructure.Validation
{
    public class NotNullOrEmptyValidator : ValidationRule
    {
        public override ValidationResult Validate(object value)
        {

            if(value is null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationFailed("value cannot be empty");
            }

            return ValidationSucces();
        }

        public override string StringRepresentation => "NotEmpty";
    }
}
