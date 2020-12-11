using System;
using System.Globalization;
using System.ComponentModel;

namespace KEI.Infrastructure
{
    [TypeConverter(typeof(PointConverter))]
    public record Point(double X, double Y)
    {
        /// <summary>
        /// Override <see cref="object.ToString"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{X},{Y}";
        }

        /// <summary>
        /// Parse <see cref="Point"/> from string
        /// throws exception for invalid string
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Point Parse(string point)
        {
            var values = point.Split(',');

            return new Point(double.Parse(values[0]), double.Parse(values[1]));
        }

        /// <summary>
        /// Tries to parse <see cref="Point"/> from string
        /// </summary>
        /// <param name="point"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool TryParse(string point, out Point p)
        {
            p = new Point(0, 0);

            var values = point.Split(',');

            if (values.Length != 2)
            {
                return false;
            }

            if (double.TryParse(values[0], out double x) &&
                double.TryParse(values[1], out double y))
            {
                p = new Point(x, y);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// <see cref="TypeConverter"/> implementation for <see cref="Point"/>
    /// </summary>
    public class PointConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value is string str
                ? Point.Parse(str)
                : base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var casted = value as Point;
            return destinationType == typeof(string) && casted != null
                ? casted.ToString()
                : base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            return value is string str
                ? Point.TryParse(str, out _)
                : base.IsValid(context, value);
        }
    }
}
