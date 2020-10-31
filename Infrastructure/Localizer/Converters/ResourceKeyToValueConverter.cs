using KEI.UI.Wpf;
using System;
using System.Globalization;

namespace Localizer.Converters
{
    public class ResourceKeyToValueConverter : BaseValueConverter<ResourceKeyToValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString().Replace("_", " ");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString().Replace(" ", "_");
        }
    }
}
