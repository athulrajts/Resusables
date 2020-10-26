using Application.Core;
using Prism.Ioc;
using System.Windows;
using System.Windows.Controls;

namespace Application.Engineering.Dialogs
{
    /// <summary>
    /// Interaction logic for LoadingOverlay.xaml
    /// </summary>
    public partial class ChangeLanguage : Window
    {
        public ISystemStatusManager StatusManager { get; set; }
        public ChangeLanguage()
        {
            InitializeComponent();
            StatusManager = ContainerLocator.Container.Resolve<ISystemStatusManager>();
            DataContext = this;
        }

        public static void Prompt() => new ChangeLanguage().ShowDialog();
    }
}
