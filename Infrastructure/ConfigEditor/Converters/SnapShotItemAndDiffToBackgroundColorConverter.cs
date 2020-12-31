using KEI.Infrastructure;
using KEI.UI.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ConfigEditor.Converters
{
    public class SnapShotItemAndDiffToBackgroundColorConverter : BaseMultiValueConverter<SnapShotItemAndDiffToBackgroundColorConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values.Length != 2)
            {
                return DependencyProperty.UnsetValue;
            }

            SnapShotItem item = values[0] as SnapShotItem;
            SnapShotDiff diff = values[1] as SnapShotDiff;

            if(item is null || diff is null)
            {
                return DependencyProperty.UnsetValue;
            }

            return item.Value.Equals(diff[item.Name].Left)
                ? Brushes.Blue
                : Brushes.Green;
            
        }
    }
}
