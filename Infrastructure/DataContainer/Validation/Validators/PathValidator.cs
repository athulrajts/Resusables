using System.IO;
using System.Xml.Serialization;

namespace KEI.Infrastructure.Validation
{
    public enum PathValidationMode
    {
        File,
        Directory
    }
    public class PathValidator : ValidationRule
    {
        private PathValidationMode mode;

        [XmlAttribute]
        public PathValidationMode Mode
        {
            get { return mode; }
            set { SetValidationProperty(ref mode, value); }
        }
        public override ValidationResult Validate(object value)
        {
            string str = value?.ToString();

            if (string.IsNullOrEmpty(str))
            {
                return ValidationFailed("Path cannot be empty");
            }

            if (Mode == PathValidationMode.File)
            {
                if (File.Exists(str) == false)
                {
                    return ValidationFailed($"File \"{str}\" does not exisit");
                }
            }
            else
            {
                if (Directory.Exists(str) == false)
                {
                    return ValidationFailed($"Directory \"{str}\" does not exisit");
                }
            }

            return ValidationSucces();
        }

        public override string StringRepresentation => Mode.ToString();
    }
}
