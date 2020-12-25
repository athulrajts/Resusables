using System;
using System.Globalization;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace KEI.Infrastructure
{

    /// <summary>
    /// Cross platform data structure to store color information
    /// </summary>
    [TypeConverter(typeof(ColorTypeConverter))]
    public record Color(byte R, byte G, byte B)
    {
        private static readonly Regex regex = new Regex("#([0-9a-fA-F]{6})", RegexOptions.Compiled);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hex"></param>
        public Color(string hex) : this(0, 0, 0)
        {
            if (Parse(hex) is Color c)
            {
                (R, G, B) = c;
            }
        }

        /// <summary>
        /// Parse <see cref="Color"/> from hex string
        /// throws <see cref="InvalidOperationException"/> if invalid hex string is given
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
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
            else
            {
                throw new InvalidOperationException("Unable to create value from value");
            }
        }

        /// <summary>
        /// Tries to parse <see cref="Color"/> from given hex string
        /// </summary>
        /// <param name="hex"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool TryParse(string hex, out Color c)
        {
            c = new Color(0, 0, 0);

            try
            {
                c = Parse(hex);

                return true;
            }
            catch { }

            return false;
        }


        /// <summary>
        /// Override <see cref="object.ToString"/> to give hex value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"#{R:X2}{G:X2}{B:X2}";
        }
    }

    /// <summary>
    /// <see cref="TypeConverter"/> implementation for <see cref="Color"/>
    /// </summary>
    public class ColorTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value is string str
                ? Color.Parse(str)
                : base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var casted = value as Color;
            return destinationType == typeof(string) && casted != null
                ? casted.ToString()
                : base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            return value is string str
                ? Color.TryParse(str, out _)
                : base.IsValid(context, value);
        }
    }
}
