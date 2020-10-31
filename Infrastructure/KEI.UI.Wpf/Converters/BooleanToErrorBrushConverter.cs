using System;
using System.Globalization;
using System.Windows.Media;

namespace KEI.UI.Wpf
{
    /// <summary>
    /// Returns <see cref="Brushes.OrangeRed"/> if bound value is false
    /// Otherwise returns <see cref="Brushes.Transparent"/>
    /// </summary>
    class BooleanToErrorBrushConverter : BaseValueConverter<BooleanToErrorBrushConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isValid)
            {
                return isValid ? Brushes.Transparent : new SolidColorBrush { Color = Colors.Red, Opacity = 0.20};
            }
            return Brushes.Transparent;
        }
    }
}
