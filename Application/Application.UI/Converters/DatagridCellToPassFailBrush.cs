using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Application.UI.Controls;
using KEI.Infrastructure.Database;
using KEI.UI.Wpf;

namespace Application.UI.Converters
{
    public class DatagridCellToPassFailBrush : ValueConverterExtension<DatagridCellToPassFailBrush>
    {
        private Brush failBrush = new SolidColorBrush { Color = Colors.OrangeRed};
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                DataGridCell dgc = (DataGridCell)value;
                System.Data.DataRowView rowView = (System.Data.DataRowView)dgc.DataContext;
                var content = rowView.Row.ItemArray[dgc.Column.DisplayIndex].ToString();
                var dbColumn = (dgc.Column as TaggedDatagridTextColumn).Tag as DatabaseColumn;

                return dbColumn.IsValid(content) ? Brushes.Transparent : failBrush;
            }
            catch (InvalidCastException)
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
