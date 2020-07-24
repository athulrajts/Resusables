using KEI.UI.Wpf;
using System;
using System.Globalization;
using System.Windows.Controls;

namespace Application.UI.Converters
{
    class DatagridRowToIndexConverter : ValueConverterExtension<DatagridRowToIndexConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DataGridRow row = value as DataGridRow;
            if (row == null)
                throw new InvalidOperationException("This converter class can only be used with DataGridRow elements.");

            return row.GetIndex() + 1;
        }
    }
}
