using System;
using System.Windows;
using System.Threading;
using System.Globalization;
using System.Collections.ObjectModel;
using Prism.Ioc;
using Prism.Unity;
using Prism.Modularity;
using KEI.Infrastructure;
using KEI.Infrastructure.Prism;
using KEI.Infrastructure.Service;
using KEI.Infrastructure.Configuration;
using KEI.UI.Wpf.ViewService;
using Application.Core;
using Application.Core.Modules;
using Application.Core.Interfaces;
using Application.UI.ViewService;
using Application.UI.AdvancedSetup;
using ApplicationShell.Commands;
using KEI.Infrastructure.Utils;
using KEI.Infrastructure.Logging;

#if DEBUG
using KEI.Infrastructure.Localizer;
#endif

namespace ApplicationShell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        public int MyProperty { get; set; }
        protected override Window CreateShell()
        {
            LoginWindow loginView = Container.Resolve<LoginWindow>();


            if (loginView.ShowDialog() == false)
            {
                Environment.Exit(0);
            }

            splash.Show();

            var mainWindow = Container.Resolve<MainWindow>();
            mainWindow.Loaded += (_, __) =>
            {
                Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                mainWindow.Activate();
            };

#if DEBUG
            mainWindow.Closing += (_, __) => LocalizationManager.Instance.WriteKeys();
#endif

            return mainWindow;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            SplashScreenLogger.Instance.Log("Initializing Application Services");

            var services = XmlHelper.Deserialize<ObservableCollection<Service>>("Configs/Services.cfg");
            if (services == null)
            {
                Current.Shutdown();
                MessageBox.Show("Unable to load Service Config");
                Environment.Exit(0);
            }

            // Register Logger
            containerRegistry.RegisterLogger(SimpleLogConfigurator.Configure()
                .WriteToFile(@"Logs\Log.slog").Create()
                .Finish());

            // Register Infrastructure Services
            containerRegistry.RegisterInfrastructureServices();
            containerRegistry.RegisterUIServices();

            // Register Services from Config File
            containerRegistry.RegisterServices(services);

            // Regster Application Related Services
            containerRegistry.RegisterSingleton<IDatabaseManager, DatabaseManager>();
            containerRegistry.RegisterSingleton<ISystemStatusManager, ApplicationViewModel>();
            containerRegistry.RegisterSingleton<IEquipment, Equipment>();
            containerRegistry.RegisterSingleton<ApplicationCommands>();

            // Register Dialogs
            containerRegistry.RegisterDialog<AdvancedSetupDialog>();
            containerRegistry.RegisterDialog<StartTestDialog>();

            Container.Resolve<IConfigManager>();
            Container.Resolve<IEquipment>();

         }



        protected override void ConfigureServiceLocator()
        {
            base.ConfigureServiceLocator();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            var moduleCatalog = new DirectoryModuleCatalog() { ModulePath = @".\Plugins" };
            moduleCatalog.AddModule(new ModuleInfo { ModuleName = typeof(ApplicationModule).Name, ModuleType = typeof(ApplicationModule).AssemblyQualifiedName });
            return moduleCatalog;
        }


        protected override void OnInitialized()
        {
            SplashScreenLogger.Instance.Log("Application Initialized");

            Container.Resolve<ISystemStatusManager>().ApplicationMode = StartupApplicationMode;

            Container.Resolve<IEquipment>().LoadRecipe("Configs/DefaultRecipe.rcp");

            Logger.Debug("Application Initialized");

            base.OnInitialized();

            splash.Close();
        }

        public ApplicationMode StartupApplicationMode = ApplicationMode.Production;
        private Application.UI.SplashScreen splash;
        private void PrismApplication_Startup(object sender, StartupEventArgs e)
        {

            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var culture = Thread.CurrentThread.CurrentCulture.Name;
            var appMode = string.Empty;

            var args = new CommandLineArguments(e.Args);

            if(args.TryGetValue("mode", ref appMode))
            {
                switch(appMode)
                {
                    case "eng": StartupApplicationMode = ApplicationMode.Engineering; break;
                }
            }

            args.TryGetValue("lang", ref culture);


            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(culture);
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(culture);

            splash = new Application.UI.SplashScreen(StartupApplicationMode);
            splash.Show();
            splash.Hide();

        }
    }
}
