using KEI.Infrastructure;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace KEI.UI.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for WinFormsPropertyGrid.xaml
    /// </summary>
    public partial class WinFormsPropertyGrid : UserControl
    {
        public WinFormsPropertyGrid()
        {
            InitializeComponent();
        }

        public object SelectedObject
        {
            get { return (object)GetValue(SelectedObjectProperty); }
            set { SetValue(SelectedObjectProperty, value); }
        }

        public static readonly DependencyProperty SelectedObjectProperty =
            DependencyProperty.Register("SelectedObject", typeof(object), typeof(WinFormsPropertyGrid), new PropertyMetadata(null, OnSelectedObjectChanged));

        private static void OnSelectedObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var wfpg = d as WinFormsPropertyGrid;

            wfpg.propertyGrid.SelectedObject = e.NewValue;

            if(e.NewValue is INotifyPropertyChanged newValue)
            {
                newValue.PropertyChanged += wfpg.Inpc_PropertyChanged;
            }
            if(e.OldValue is INotifyPropertyChanged oldValue)
            {
                oldValue.PropertyChanged -= wfpg.Inpc_PropertyChanged;
            }
        }

        private void Inpc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Incase of datacontainer, we don't want to refresh in case of inner datacontainer property changes
            // since they are not visible
            if(e.PropertyName.Split('.').Length == 1)
            {
                propertyGrid.Refresh();
            }
        }
    }
}
