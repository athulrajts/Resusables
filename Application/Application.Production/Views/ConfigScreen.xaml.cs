using KEI.Infrastructure.Prism;
using System.Windows.Controls;

namespace Application.Production.Views
{
    /// <summary>
    /// Interaction logic for SubView1.xaml
    /// </summary>
    [RegisterForNavigation]
    public partial class ConfigScreen : UserControl
    {
        public ConfigScreen()
        {
            InitializeComponent();
            listbox.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void TreeView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            (treeView.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem).IsSelected = true;
        }
    }
}
