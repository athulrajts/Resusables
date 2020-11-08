using System.Xml.Serialization;
using KEI.Infrastructure.Validation.Attributes;

namespace KEI.Infrastructure.Validation
{
    public class LengthValidator : ValidationRule
    {
        private int min;
        private int max;

        [XmlAttribute]
        [Positive]
        public int Min
        {
            get { return min; }
            set { SetValidationProperty(ref min, value); }
        }

        [XmlAttribute]
        [Positive]
        public int Max
        {
            get { return max; }
            set { SetValidationProperty(ref max, value); }
        }
        public override ValidationResult Validate(object value)
        {
            CurrentResult = new ValidationResult(true);

            string str = value as string;

            if(str == null)
            {
                CurrentResult = new ValidationResult(false, $"Value must be string");
                return CurrentResult;
            }

            if(Max == Min && str.Length != Max)
            {
                CurrentResult = new ValidationResult(false, $"Length must be equal to {Max}");
                return CurrentResult;
            }

            string commonErrMsg = $"Length should be between ({Min} - {Max}) Characters";

            if(str.Length > Max && Max >= 0)
            {
                CurrentResult = new ValidationResult(false, Min >= 0 ? commonErrMsg : $"Length must be lesser than {Max}");
                return CurrentResult;
            }


            if(str.Length < Min && Min >= 0)
            {
                CurrentResult = new ValidationResult(false, Max >= 0 ? commonErrMsg : $"Length must be greater than {Min}");
                return CurrentResult;
            }

            return CurrentResult;
        }

        public override string StringRepresentation
        {
            get
            {
                if (Min < 0 )
                    return $"MaxLength - {Max}";
                else if (Max == int.MaxValue)
                    return $"MinLength - {Min}";
                else if (max == min)
                    return $"Length = {Max}";
                else if(Min != int.MinValue && Max != int.MaxValue)
                    return $"Length[{Min} - {Max}]";

                return string.Empty;
            }
        }
    }
}
