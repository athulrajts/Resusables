using KEI.UI.Wpf.ViewService;
using System.Windows.Controls;

namespace Application.UI.ViewService
{
    /// <summary>
    /// Interaction logic for StartTestDialog.xaml
    /// </summary>
    public partial class StartTestDialog : UserControl
    {
        public StartTestDialog()
        {
            InitializeComponent();
            DataContext = CommonServiceLocator.ServiceLocator.Current.GetInstance<StartTestDialogViewModel>();

            Loaded += StartTestDialog_Loaded;
        }

        private void StartTestDialog_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            trackingId.Focus();
        }

        private void TextBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is TextBox tb)
                tb.SelectAll();
        }
    }
}
