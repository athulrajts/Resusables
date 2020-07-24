using System.Windows.Controls;

namespace KEI.UI.Wpf.ViewService
{
    /// <summary>
    /// Interaction logic for ConfigsChangedDialog.xaml
    /// </summary>
    public partial class ConfigsChangedDialog : UserControl
    {
        public ConfigsChangedDialog()
        {
            InitializeComponent();
            DataContext = CommonServiceLocator.ServiceLocator.Current.GetInstance<ConfigsChangedDialogViewModel>();
        }
    }
}
