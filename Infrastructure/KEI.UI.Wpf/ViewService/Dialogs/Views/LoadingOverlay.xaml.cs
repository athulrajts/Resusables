using System.Windows.Input;

namespace KEI.UI.Wpf.ViewService.Dialogs
{
    /// <summary>
    /// Interaction logic for LoadingOverlay.xaml
    /// </summary>
    public partial class LoadingOverlay : DialogWindow
    {
        public LoadingOverlay()
        {
            InitializeComponent();
            Cursor = Cursors.Wait;
        }

    }
}
