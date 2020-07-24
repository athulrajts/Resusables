using System.Xml.Serialization;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KEI.Infrastructure.Validation
{
    [XmlRoot(IsNullable = false, ElementName = "Validations")]
    public class ValidatorGroup : ValidationRule
    {
        [XmlAttribute("Cascade")]
        public bool CascadeValidations { get; set; }

        [XmlElement(IsNullable = false, ElementName = "Rule")]
        [XmlElement(IsNullable = false, Type = typeof(RangeValidator), ElementName = nameof(RangeValidator))]
        [XmlElement(IsNullable = false, Type = typeof(PathValidator), ElementName = nameof(PathValidator))]
        [XmlElement(IsNullable = false, Type = typeof(LengthValidator), ElementName = nameof(LengthValidator))]
        [XmlElement(IsNullable = false, Type = typeof(NumberSignValidator), ElementName = nameof(NumberSignValidator))]
        [XmlElement(IsNullable = false, Type = typeof(MustBeOneOfValidator), ElementName = nameof(MustBeOneOfValidator))]
        [XmlElement(IsNullable = false, Type = typeof(NotNullOrEmptyValidator), ElementName = nameof(NotNullOrEmptyValidator))]
        [XmlElement(IsNullable = false, Type = typeof(LinearInequalityValidator), ElementName = nameof(LinearInequalityValidator))]
        public ObservableCollection<ValidationRule> Rules { get; set; } = new ObservableCollection<ValidationRule>();

        public override ValidationResult Validate(object value)
        {
            var isValid = true;
            var failedValidation = new List<string>();
            foreach (var validator in Rules)
            {
                var result = validator.Validate(value);

                if (!result.IsValid)
                {
                    isValid = false;

                    if (!CascadeValidations)
                    {
                        return new ValidationResult(false, result.ErrorMessage);
                    }
                    else
                    {
                        failedValidation.Add(validator.StringRepresentation);
                    }
                }
            }

            return isValid ? new ValidationResult(true) : new ValidationResult(false, $"{string.Join(",", failedValidation)} failed");
        }

        public ValidatorGroup()
        {

        }

        public ValidatorGroup(bool cascadeValidations = false)
        {
            CascadeValidations = cascadeValidations;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
                return false;

            var other = obj as ValidatorGroup;

            if (Rules.Count != other.Rules.Count)
                return false;

            for (int i = 0; i < Rules.Count; i++)
            {
                if (!Rules[i].Equals(other.Rules[i]))
                    return false;
            }

            return true;

        }

        public override int GetHashCode()
        {
            var hashCode = 1740439574;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ObservableCollection<ValidationRule>>.Default.GetHashCode(Rules);
            return hashCode;
        }

        public static implicit operator ValidatorGroup(ValidationBuilder builder) => builder.Validator;
    }
}
