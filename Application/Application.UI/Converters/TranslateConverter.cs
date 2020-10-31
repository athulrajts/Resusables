using KEI.UI.Wpf;
using System;
using System.Globalization;

namespace Application.UI.Converters
{
    public class TranslateConverter : BaseValueConverter<TranslateConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = 0;
            double param = 0;
            if (double.TryParse(value.ToString(), out val))
            {
                if(double.TryParse(parameter.ToString(), out param))
                {
                    return val + param;
                }
                return val;
            }

            return value;
        }
    }
}
