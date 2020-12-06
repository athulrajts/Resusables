using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace KEI.Infrastructure.Validation
{
    public class TypeValidator : ValidationRule
    {
        public string TypeString { get; set; } = typeof(int).FullName;

        [XmlIgnore]
        public Type Type
        {
            get => string.IsNullOrEmpty(TypeString) ? null : Type.GetType(TypeString);
            set => TypeString = value?.AssemblyQualifiedName;
        }

        public override ValidationResult Validate(object value)
        {
            if(Type == null)
                return new ValidationResult(true);

            TypeConverter converter = TypeDescriptor.GetConverter(Type);

            if (converter.IsValid(value?.ToString()) == false)
            {
                return ValidationFailed($"{value} is not convertable to {Type.FullName}");
            }

            return ValidationSucces();
        }

        public override string StringRepresentation => $"{Type.Name}";
    }
}
