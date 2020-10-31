using KEI.Infrastructure.Logging;
using KEI.UI.Wpf.ViewService;
using Prism.Ioc;
using System.Windows;
using TCPClient.ViewModels;
using TCPClient.Views;

namespace TCPClient
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

            containerRegistry.RegisterSingleton<TCPClientWindowViewModel>();
        }

        private void PrismApplication_Exit(object sender, ExitEventArgs e)
        {
            Container.Resolve<TCPClientWindowViewModel>().DisconnectCommand.Execute();
        }
    }
}
