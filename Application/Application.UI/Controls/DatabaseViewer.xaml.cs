using System.Data;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using KEI.Infrastructure.Database;

namespace Application.UI.Controls
{
    /// <summary>
    /// Interaction logic for DatabaseViewer.xaml
    /// </summary> 
    public partial class DatabaseViewer : UserControl
    {
        public DataTable Data
        {
            get { return (DataTable)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(DataTable), typeof(DatabaseViewer), new PropertyMetadata(null, OnDataChanged));

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dbv = d as DatabaseViewer;

            if (e.NewValue is DataTable dt)
            {
                dbv.dataGrid.Columns.Clear();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    TaggedDatagridTextColumn dgtc = new TaggedDatagridTextColumn();
                    if (dt.Columns[i] is TaggedDataColumn dc)
                    {
                        dgtc.Binding = new Binding(dc.ColumnName);
                        dgtc.Header = dc.Caption;
                        dgtc.Width = DataGridLength.Auto;
                        dgtc.IsReadOnly = true;
                        dgtc.Tag = dc.Tag;
                        dbv.dataGrid.Columns.Add(dgtc); 
                    }
                }
            }
        }

        public DatabaseViewer()
        {
            InitializeComponent();
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }

    public class TaggedDatagridTextColumn : DataGridTextColumn
    {
        public object Tag { get; set; }
    }
}
