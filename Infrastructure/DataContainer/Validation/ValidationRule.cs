using System;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Runtime.CompilerServices;

namespace KEI.Infrastructure.Validation
{
    public interface IValidationRule
    {
        ValidationResult Validate(object value);
    }

    [XmlInclude(typeof(LinearInequalityValidator))]
    [XmlInclude(typeof(RangeValidator))]
    [XmlInclude(typeof(LengthValidator))]
    [XmlInclude(typeof(NumberSignValidator))]
    [XmlInclude(typeof(PathValidator))]
    [XmlInclude(typeof(NotNullOrEmptyValidator))]
    [XmlInclude(typeof(MustBeOneOfValidator))]
    [XmlInclude(typeof(TypeValidator))]
    [XmlInclude(typeof(ValidatorGroup))]
    public abstract class ValidationRule : BindableObject, IValidationRule
    {
        public abstract ValidationResult Validate(object value);

        protected ValidationResult CannotBeNull()
        {
            CurrentResult = new ValidationResult(false, "Value cannot be null or empty");
            return CurrentResult;
        }
        protected ValidationResult ValidationSucces()
        {
            CurrentResult = new ValidationResult(true);
            return CurrentResult;
        }

        private ValidationResult currentResult;

        [XmlIgnore]
        [Browsable(false)]
        public ValidationResult CurrentResult
        {
            get { return currentResult; }
            set { SetProperty(ref currentResult, value); }
        }   

        public override bool Equals(object obj)
        {
            if (GetType() != obj.GetType())
                return false;

            var props = GetType().GetProperties();


            foreach (var prop in props)
            {
                var lval = prop.GetValue(this);
                var rval = prop.GetValue(obj);

                if (typeof(IConvertible).IsAssignableFrom(prop.PropertyType))
                {
                    if (lval.ToString() != rval.ToString())
                    {
                        return false;
                    }
                }
                else if (!lval?.Equals(rval) == false)
                    return false;
            }

            return true;
        }

        protected ValidationResult ValidationFailed(string message)
        {
            CurrentResult = new ValidationResult(false, message);
            return CurrentResult;
        }

        [Browsable(false)]
        public virtual string StringRepresentation => string.Empty;

        protected bool SetValidationProperty<T>(ref T store, T value, [CallerMemberName] string property ="")
        {
            var propertySet = SetProperty(ref store, value, property);

            if(propertySet)
            {
                RaisePropertyChanged(nameof(StringRepresentation));
            }

            return propertySet;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }

        public ValidationResult(bool isValid, string err = "")
        {
            IsValid = isValid;
            ErrorMessage = err;
        }
    }
}
