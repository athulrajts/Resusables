using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace KEI.UI.Wpf.Converters
{
    /// <summary>
    /// Convert <see cref="Infrastructure.Color"/> to <see cref="System.Windows.Media.Color"/> and back
    /// </summary>
    public class ColorConverter : BaseValueConverter<ColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Infrastructure.Color c)
            {
                return Color.FromRgb(c.R, c.G, c.B);
            }

            return DependencyProperty.UnsetValue;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color c)
            {
                return new Infrastructure.Color(c.R, c.G, c.B);
            }

            return DependencyProperty.UnsetValue;
        }
    }

    public class ColorToBrushConverter : BaseValueConverter<ColorToBrushConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Infrastructure.Color c)
            {
                return new SolidColorBrush(Color.FromRgb(c.R, c.G, c.B));
            }

            return DependencyProperty.UnsetValue;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush b)
            {
                return new Infrastructure.Color(b.Color.R, b.Color.G, b.Color.B);
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
