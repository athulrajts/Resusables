using KEI.Infrastructure.Prism;
using System.Windows.Controls;

namespace Application.Engineering.Layout.Sections.Views
{
    /// <summary>
    /// Interaction logic for DatabaseSection.xaml
    /// </summary>
    [RegisterWithRegion("Database")]
    public partial class DatabaseSection : UserControl
    {
        public DatabaseSection()
        {
            InitializeComponent();
        }

        private void DatabaseViewer_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
