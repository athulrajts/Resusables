using KEI.Infrastructure.Logging;
using KEI.UI.Wpf.ViewService;
using LogViewer.Views;
using Prism.Ioc;
using Prism.Unity;
using System.Windows;

namespace LogViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell() => Container.Resolve<LogViewerWindow>();

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterLogger("MongolianTest.txt");
            containerRegistry.RegisterUIServices();

            Logger.Debug("Let there be light !");
        }
    }
}
