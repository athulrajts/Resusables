using KEI.Infrastructure.Logging;
using KEI.UI.Wpf.ViewService;
using Prism.Ioc;
using System.Windows;
using TCPIPClient.Views;

namespace TCPIPClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell() => Container.Resolve<TCPClientWindow>();

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterConsoleLogger();
            containerRegistry.RegisterUIServices();
        }
    }
}
