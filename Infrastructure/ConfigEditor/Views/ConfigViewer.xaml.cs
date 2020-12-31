using ConfigEditor.Models;
using ConfigEditor.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ConfigEditor.Views
{
    /// <summary>
    /// Interaction logic for ConfigViewer.xaml
    /// </summary>
    public partial class ConfigViewer : UserControl
    {
        public ConfigViewer()
        {
            InitializeComponent();
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if(DataContext is ConfigViewerViewModel vm)
            {
                vm.SelectedNode = (TreeNodeModel)e.NewValue;
            }
        }
    }
}
