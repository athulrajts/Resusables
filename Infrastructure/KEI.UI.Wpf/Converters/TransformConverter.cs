using System;
using System.Globalization;

namespace KEI.UI.Wpf.Converters
{
    public class TransformConverter : ValueConverterExtension<TransformConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(double.TryParse(value?.ToString(), out double number) &&
                double.TryParse(parameter?.ToString(), out double offset))
            {
                return number + offset;
            }

            return value;
        }
    }
}
