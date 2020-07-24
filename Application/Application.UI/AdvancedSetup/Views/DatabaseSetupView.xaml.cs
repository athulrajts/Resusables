using Application.UI.AdvancedSetup.ViewModels;
using System.Windows.Controls;

namespace Application.UI.AdvancedSetup.Views
{
    /// <summary>
    /// Interaction logic for DatabaseSetup.xaml
    /// </summary>
    public partial class DatabaseSetupView : UserControl
    {
        public DatabaseSetupView()
        {
            InitializeComponent();
            DataContext = new DatabaseSetupViewModel();
        }
    }
}
