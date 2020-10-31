using System;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Application.UI.Controls;
using KEI.Infrastructure.Database;
using KEI.UI.Wpf;

namespace Application.UI.Converters
{
    public class DatagridCellToPassFailBrush : BaseValueConverter<DatagridCellToPassFailBrush>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                DataGridCell dgc = (DataGridCell)value;
                DataRowView rowView = (DataRowView)dgc.DataContext;

                return string.IsNullOrEmpty(rowView.Row.GetColumnError(dgc.Column.DisplayIndex))
                    ? Brushes.Black 
                    : Brushes.Red;
            }
            catch (InvalidCastException)
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
