using System;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Ioc;
using Prism.Unity;
using Prism.Modularity;
using KEI.Infrastructure;
using KEI.Infrastructure.Prism;
using KEI.Infrastructure.Utils;
using KEI.Infrastructure.Types;
using KEI.Infrastructure.Logging;
using KEI.Infrastructure.Service;
using KEI.Infrastructure.Database;
using KEI.Infrastructure.Configuration;
using KEI.UI.Wpf.ViewService;
using Application.Core;
using Application.Core.Camera;
using Application.Core.Modules;
using Application.Core.Interfaces;
using Application.UI;
using Application.UI.ViewService;
using Application.UI.AdvancedSetup;
using ApplicationShell.Commands;
using KEI.Infrastructure.Server;

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

            // Register Logger
            containerRegistry.RegisterLogger(SimpleLogConfigurator.Configure()
                .WriteToFile(PathUtils.GetPath("Logs/Log.slog"))
                .Finish());

            // Register Infrastructure Services
            containerRegistry.RegisterInfrastructureServices();
            containerRegistry.RegisterUIServices();

            // Register Services from Config File
            containerRegistry.RegisterServices(GetServices());

            // Initialize Native
            //NativeInitializer.SetLogger(Container.Resolve<ILogManager>());
            //NativeInitializer.SetViewService(Container.Resolve<IViewService>());

            // Regster Application Related Services
            containerRegistry.RegisterSingleton<IDatabaseManager, DatabaseManager>();
            containerRegistry.RegisterSingleton<ISystemStatusManager, ApplicationViewModel>();
            containerRegistry.RegisterSingleton<IEquipment, Equipment>();
            containerRegistry.RegisterSingleton<ApplicationCommands>();

            // Register Dialogs
            containerRegistry.RegisterDialog<AdvancedSetupDialog>();
            containerRegistry.RegisterDialog<StartTestDialog>();

            // Resolve necessary types
            Container.Resolve<IConfigManager>();
            Container.Resolve<IEquipment>();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            var moduleCatalog = new DirectoryModuleCatalog() { ModulePath = @".\Plugins" };
            moduleCatalog.AddModule<ApplicationModule>();
            moduleCatalog.AddModule<ApplicationUIModule>();
            return moduleCatalog;
        }


        protected override void OnInitialized()
        {
            SplashScreenLogger.Instance.Log("Application Initialized");

            Container.Resolve<ISystemStatusManager>().ApplicationMode = StartupApplicationMode;

            Container.Resolve<IEquipment>().LoadRecipe(PathUtils.GetPath("Configs/DefaultRecipe.rcp"));

            Logger.Debug("Application Initialized");

            ServiceManager.InitializeServices();

            splash.Close();

            base.OnInitialized();
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

        private IEnumerable<ServiceInfo> GetServices()
        {
            var serviceConfigPath = PathUtils.GetPath("Configs/Services.cfg");

            var selectedService = XmlHelper.DeserializeFromFile<ObservableCollection<ServiceInfo>>(serviceConfigPath);

            if (selectedService == null)
            {
                selectedService = GetDefaultServices();
                XmlHelper.SerializeToFile(new ObservableCollection<ServiceInfo>(selectedService), serviceConfigPath);
            }

            return selectedService;
        }

        private ObservableCollection<ServiceInfo> GetDefaultServices()
        {
            return new ObservableCollection<ServiceInfo>
            {
                new ServiceInfo
                {
                    ServiceType = typeof(IApplicationViewService),
                    ImplementationType = typeof(ApplicationViewService),
                    Name = "View Service",
                },
                new ServiceInfo
                {
                    ServiceType = typeof(IVisionProcessor),
                    ImplementationType = typeof(VisionProcessor),
                    Name = "Vision"
                },
                new ServiceInfo
                {
                    ServiceType = typeof(IGreedoCamera),
                    ImplementationType = typeof(SimulatedCamera),
                    Name = "Camera"
                },
                new ServiceInfo
                {
                    ServiceType = typeof(IDatabaseReader),
                    ImplementationType = typeof(CSVDatabaseReader),
                    Name = "Database Reader"
                },
                new ServiceInfo
                {
                    ServiceType = typeof(IDatabaseWritter),
                    ImplementationType = typeof(CSVDatabaseWritter),
                    Name = "Database Writter"
                },
            };
        }

        private void PrismApplication_Exit(object sender, ExitEventArgs e)
        {
            Container.Resolve<IServer>().StopServer();
        }
    }
}
