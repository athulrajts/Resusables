using KEI.Infrastructure.Logging;
using KEI.Infrastructure.Prism;
using KEI.UI.Wpf.ViewService;
using Prism.Ioc;
using System.Windows;

namespace ServiceEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<ServiceEditor.MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterConsoleLogger();
            containerRegistry.RegisterInfrastructureServices();
            containerRegistry.RegisterUIServices();
        }
    }
}
