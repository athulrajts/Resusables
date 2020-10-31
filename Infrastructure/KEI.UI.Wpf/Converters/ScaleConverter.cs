using System;
using System.Globalization;

namespace KEI.UI.Wpf.Converters
{
    /// <summary>
    /// Takes in anything that can be parsed to <see cref="double"/>
    /// Returns input * parameter
    /// </summary>
    public class ScaleConverter : BaseValueConverter<ScaleConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(double.TryParse(value?.ToString(), out double number) &&
                double.TryParse(parameter?.ToString(), out double factor))
            {
                return number * factor;
            }

            return value;
        }
    }
}
