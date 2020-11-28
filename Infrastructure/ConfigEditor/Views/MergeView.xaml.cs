using KEI.Infrastructure;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace ConfigEditor.Views
{
    /// <summary>
    /// Interaction logic for MergeView.xaml
    /// </summary>
    public partial class MergeView : UserControl
    {
        public MergeView()
        {
            InitializeComponent();
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }

    public class DataContainerToDictionaryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IPropertyContainer dc)
                return dc.ToDictionary();

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
