using CommonServiceLocator;
using KEI.Infrastructure.Events;
using Prism.Events;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KEI.UI.Wpf.ViewService.Dialogs
{
    /// <summary>
    /// Interaction logic for LoadingOverlay.xaml
    /// </summary>
    public partial class LoadingOverlay : UserControl
    {
        public LoadingOverlay()
        {
            InitializeComponent();
            DataContext = CommonServiceLocator.ServiceLocator.Current.GetInstance<LoadingOverlayViewModel>();
            Cursor = Cursors.Wait;
        }

    }
}
