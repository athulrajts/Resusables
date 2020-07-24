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
                CurrentResult = new ValidationResult(false, "Path cannot be empty");
                return CurrentResult;
            }

            if (Mode == PathValidationMode.File)
            {
                if (!File.Exists(str))
                {
                    CurrentResult = new ValidationResult(false, $"Path \"{str}\" does not exisit");
                    return CurrentResult;
                }
            }
            else
            {
                if (!Directory.Exists(str))
                {
                    CurrentResult = new ValidationResult(false, $"Path \"{str}\" does not exisit");
                    return CurrentResult;
                }
            }

            return ValidationSucces();
        }

        public override string StringRepresentation => Mode.ToString();
    }
}
