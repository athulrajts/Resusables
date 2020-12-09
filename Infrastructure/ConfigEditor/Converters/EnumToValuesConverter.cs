using KEI.UI.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ConfigEditor.Converters
{
    public class EnumToValuesConverter : BaseValueConverter<EnumToValuesConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Enum e)
            {
                return new List<string>(Enum.GetNames(e.GetType()));
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
