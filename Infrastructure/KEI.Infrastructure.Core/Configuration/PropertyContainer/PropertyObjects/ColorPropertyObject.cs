using System;

namespace KEI.Infrastructure
{
    /// <summary>
    /// PropertyObject implementation to store Color
    /// Application UI should convert to UI color object using RGB values
    /// </summary>
    public class ColorPropertyObject : PropertyObject
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="color"></param>
        public ColorPropertyObject(string name, Color color)
        {
            Name = name;
            Value = color;
        }

        public override string Type => "color";


        private Color _value;
        public Color Value
        {
            get { return _value; }
            set
            {
                if (_value?.Equals(value) == true)
                {
                    return;
                }

                _value = value;
                stringValue = $"#{_value.R:X2}{_value.G:X2}{_value.B:X2}";

                RaisePropertyChanged(nameof(Value));
                RaisePropertyChanged(nameof(StringValue));
            }
        }


        public override Type GetDataType()
        {
            return typeof(Color);
        }

        public override object GetValue()
        {
            return Value;
        }

        public override bool SetValue(object value)
        {
            if (value is not Color)
            {
                return false;
            }

            Value = (Color)value;

            return true;
        }

        protected override void OnStringValueChanged(string value)
        {
            try
            {
                if (Color.Parse(value) is Color c)
                {
                    Value = c;
                    RaisePropertyChanged(nameof(value));
                }
            }
            catch { }
        }
    }
}
