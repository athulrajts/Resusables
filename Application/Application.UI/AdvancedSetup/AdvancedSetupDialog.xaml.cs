using System.Windows.Controls;

namespace Application.UI.AdvancedSetup
{
    /// <summary>
    /// Interaction logic for AdvancedSetup.xaml
    /// </summary>
    public partial class AdvancedSetupDialog : UserControl
    {
        public AdvancedSetupDialog()
        {
            InitializeComponent();
            DataContext = CommonServiceLocator.ServiceLocator.Current.GetInstance<AdvancedSetupDialogViewModel>();
        }
    }
}
