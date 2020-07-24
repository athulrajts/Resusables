using KEI.Infrastructure.Database;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
                    TaggedDataColumn dc = dt.Columns[i] as TaggedDataColumn;
                    var columnName = dc.ColumnName;
                    var columnCaption = dc.Caption;
                    dgtc.Binding = new Binding(columnName);
                    dgtc.Header = columnCaption;
                    dgtc.Width = DataGridLength.Auto;
                    dgtc.IsReadOnly = true;
                    dgtc.Tag = dc.Tag;
                    dbv.dataGrid.Columns.Add(dgtc);
                }
            }
        }
        public DatabaseSchema Schema
        {
            get { return (DatabaseSchema)GetValue(SchemaProperty); }
            set { SetValue(SchemaProperty, value); }
        }

        public static readonly DependencyProperty SchemaProperty =
            DependencyProperty.Register("Schema", typeof(DatabaseSchema), typeof(DatabaseViewer), new PropertyMetadata(null));

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
