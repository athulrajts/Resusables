using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace KEI.UI.Wpf.Converters
{
    public class TransformConverter : ValueConverterExtension<TransformConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(double.TryParse(value?.ToString(), out double v) &&
                double.TryParse(parameter?.ToString(), out double p))
            {
                return p + v;
            }

            return value;
        }
    }
}
