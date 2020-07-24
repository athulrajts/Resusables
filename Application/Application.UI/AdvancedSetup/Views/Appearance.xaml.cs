using Application.UI.AdvancedSetup.ViewModels;
using CommonServiceLocator;
using System.Windows.Controls;

namespace Application.UI.AdvancedSetup.Views
{
    /// <summary>
    /// Interaction logic for Appearance.xaml
    /// </summary>
    public partial class Appearance : UserControl
    {
        public Appearance()
        {
            InitializeComponent();
            DataContext = ServiceLocator.Current.GetInstance<AppearanceViewModel>();
        }
    }
}
