using KEI.UI.Wpf;
using System;
using System.Globalization;
using System.Windows.Media;

namespace Application.UI.Converters
{
    public class BooleanToPassFailBrushConverter : BaseValueConverter<BooleanToPassFailBrushConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? Brushes.Green : Brushes.OrangeRed;
            }

            return Brushes.DarkGray;
        }
    }
}
