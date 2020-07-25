using System;
using System.Globalization;
using System.Windows;

namespace KEI.UI.Wpf.Converters
{
    /// <summary>
    /// Returns <see cref="Visibility.Visible"/> if bound value is true
    /// Returns <see cref="Visibility.Collapsed"/> otherwise
    /// Add Parameter of any value to invert the above logic
    /// </summary>
    public class BooleanToVisibilityConverter : ValueConverterExtension<BooleanToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool val)
            {
                bool invert = false;
                if (parameter != null)
                    invert = true;

                return (invert) ? (val) ? Visibility.Collapsed : Visibility.Visible : (val) ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

    }
}
