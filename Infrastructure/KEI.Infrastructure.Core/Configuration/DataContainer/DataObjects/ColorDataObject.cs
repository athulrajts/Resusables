using System;
using System.Text.RegularExpressions;

namespace KEI.Infrastructure
{
    /// <summary>
    /// DataObject implmementation to store Color data
    /// </summary>
    internal class ColorDataObject : DataObject
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="color"></param>
        public ColorDataObject(string name, Color color)
        {
            Name = name;
            Value = color;
        }

        /// <summary>
        /// Implmentation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => "color";

        /// <summary>
        /// Value
        /// </summary>
        private Color _value;
        public Color Value
        {
            get { return _value; }
            set
            {
                if (_value == value)
                {
                    return;
                }

                _value = value;
                stringValue = _value.ToString();

                RaisePropertyChanged(nameof(Value));
                RaisePropertyChanged(nameof(StringValue));
            }
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.GetDataType"/>
        /// </summary>
        /// <returns></returns>
        public override Type GetDataType()
        {
            return typeof(Color);
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.GetValue"/>
        /// </summary>
        /// <returns></returns>
        public override object GetValue()
        {
            return Value;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.SetValue(object)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool SetValue(object value)
        {
            if (value is not Color)
            {
                return false;
            }

            Value = (Color)value;

            return true;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.OnStringValueChanged(string)"/>
        /// </summary>
        /// <param name="value"></param>
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

    /// <summary>
    /// Cross platform data structure to store color information
    /// </summary>
    public record Color(byte R, byte G, byte B)
    {
        private static readonly Regex regex = new Regex("#([0-9a-fA-F]{6})", RegexOptions.Compiled);

        public static Color Parse(string hex)
        {
            var result = regex.Match(hex);

            if (result.Success)
            {
                string hexMatch = result.Groups[1].Value;

                byte r = Convert.ToByte(hexMatch.Substring(0, 2), 16);
                byte g = Convert.ToByte(hexMatch.Substring(2, 2), 16);
                byte b = Convert.ToByte(hexMatch.Substring(4, 2), 16);
                return new Color(r, g, b);
            }

            return null;
        }

        public Color(string hex) : this(0,0,0)
        {
            if(Parse(hex) is Color c)
            {
                R = c.R;
                G = c.G;
                B = c.B;
            }
        }

        public override string ToString()
        {
            return $"#{R:X2}{G:X2}{B:X2}";
        }
    }
}
