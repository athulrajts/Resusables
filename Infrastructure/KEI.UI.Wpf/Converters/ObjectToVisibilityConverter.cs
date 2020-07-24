using System;
using System.Collections;
using System.Globalization;
using System.Windows;

namespace KEI.UI.Wpf.Converters
{
    /// <summary>
    /// If the Object is null returns <see cref="Visibility.Collapsed"/>
    /// If the Object implements <see cref="ICollection"/> and <see cref="ICollection.Count"/> is 0; returns <see cref="Visibility.Collapsed"/>
    /// Otherwise returns <see cref="Visibility.Visible"/>
    /// </summary>
    public class ObjectToVisibilityConverter : ValueConverterExtension<ObjectToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return Visibility.Collapsed;
            else if (value is string s)
                return string.IsNullOrEmpty(s) ? Visibility.Collapsed : Visibility.Visible;
            else if (value is ICollection list && list.Count == 0)
                return Visibility.Collapsed;

            return Visibility.Visible;
        }
    }
}
