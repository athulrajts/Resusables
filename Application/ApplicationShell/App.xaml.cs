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
using System.Collections.Generic;
using KEI.Infrastructure.Types;
using Application.UI;
using Application.Core.Camera;
using KEI.Infrastructure.Database;
using System.Linq;

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
                .WriteToFile(PathUtils.GetPath("Logs/Log.slog")).Create()
                .Finish());

            // Register Infrastructure Services
            containerRegistry.RegisterInfrastructureServices();
            containerRegistry.RegisterUIServices();

            // Register Services from Config File
            containerRegistry.RegisterServices(GetServices());

            // Initialize Native
            NativeInitializer.SetLogger(Container.Resolve<ILogManager>());
            NativeInitializer.SetViewService(Container.Resolve<IViewService>());

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



        protected override void ConfigureServiceLocator()
        {
            base.ConfigureServiceLocator();
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

        private IEnumerable<Service> GetServices()
        {
            var serviceConfigPath = PathUtils.GetPath("Configs/Services.cfg");
            List<Service> selectedService = XmlHelper.Deserialize<ObservableCollection<Service>>(serviceConfigPath)?.ToList();
            List<Service> defaultServices = GetDefaultServices();
            if (selectedService == null)
            {
                selectedService = defaultServices;

                XmlHelper.Serialize(new ObservableCollection<Service>(selectedService), serviceConfigPath);
            }
            else
            {
                var modifications = new List<Action>();

                foreach (var service in defaultServices)
                {
                    if(selectedService.Find(x => x.Name == service.Name) is null)
                    {
                        modifications.Add(() => selectedService.Add(service));
                    }
                }

                foreach (var service in selectedService)
                {
                    if(defaultServices.Find(x => x.Name == service.Name) is null)
                    {
                        modifications.Add(() => selectedService.Remove(service));
                    }
                }

                if (modifications.Count > 0)
                {
                    modifications.ForEach(x => x.Invoke());
                    XmlHelper.Serialize(new ObservableCollection<Service>(selectedService), serviceConfigPath);
                }
            }

            return selectedService;
        }

        private List<Service> GetDefaultServices()
        {
            return new List<Service>
            {
                new Service
                {
                    ServiceType = new TypeInfo(typeof(IApplicationViewService)),
                    ImplementationType = new TypeInfo(typeof(ApplicationViewService)),
                    Name = "View Service"
                },
                new Service
                {
                    ServiceType = new TypeInfo(typeof(IVisionProcessor)),
                    ImplementationType = new TypeInfo(typeof(VisionProcessor)),
                    Name = "Vision"
                },
                new Service
                {
                    ServiceType = new TypeInfo(typeof(IGreedoCamera)),
                    ImplementationType = new TypeInfo(typeof(SimulatedCamera)),
                    Name = "Camera"
                },
                new Service
                {
                    ServiceType = new TypeInfo(typeof(IDatabaseReader)),
                    ImplementationType = new TypeInfo(typeof(CSVDatabaseReader)),
                    Name = "Database Reader"
                },
                new Service
                {
                    ServiceType = new TypeInfo(typeof(IDatabaseWritter)),
                    ImplementationType = new TypeInfo(typeof(CSVDatabaseWritter)),
                    Name = "Database Writter"
                },
            };
        }
    }
}
