using System.Linq;
using System.Reflection;
using Prism.Ioc;
using Prism.Events;
using Prism.Regions;
using Prism.Modularity;
using KEI.Infrastructure;
using KEI.Infrastructure.Prism;
using Application.Core;
using Application.Production.Themes;
using Application.Production.Screen;
using Application.Production.ViewModels;
using Module = KEI.Infrastructure.Prism.Module;

namespace Application.Production
{

    [ModuleDependency("ApplicationModule")]
    public class ProductionModeModule : Module
    {
        public ProductionModeModule(IRegionManager regionManager, IEventAggregator eventAggregator) : base(regionManager, eventAggregator) { }

        public override void OnInitialized(IContainerProvider containerProvider)
        {
            base.OnInitialized(containerProvider);

            // Add screens marked with ScreenAttribute to screen config so that, they could be
            // Added when needed.
            var screenConfig = containerProvider.Resolve<ScreenConfig>();
            AddScreensInAssembly(screenConfig);

            containerProvider.Resolve<ThemeManager>();
            containerProvider.Resolve<ProductionViewModel>().PopulateScreens();
            containerProvider.Resolve<TitleBarViewModel>().AttachedListners();
        }

        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            System.Windows.Application.Current.Resources.MergedDictionaries.Add(new System.Windows.ResourceDictionary
            {
                Source = new System.Uri("pack://application:,,,/Application.Production;component/Themes/ThemeStyles.xaml")
            });

            containerRegistry.RegisterSingleton<GeneralPreferences>();
            containerRegistry.RegisterSingleton<ThemeManager>();
            containerRegistry.RegisterSingleton<ScreenConfig>();
            containerRegistry.RegisterSingleton<ProductionViewModel>();

            base.RegisterTypes(containerRegistry);
        }

        /// <summary>
        /// Loop through types marked with ScreenAttribute
        /// and add them to screen config
        /// </summary>
        /// <param name="config">Screen DataContainer object</param>
        private void AddScreensInAssembly(ScreenConfig config)
        {
            var screens = _assemblyTypes
                .Where(t => t.GetCustomAttributes<ScreenAttribute>()
                .Count() > 0);

            foreach (var t in screens)
            {
                var attribute = t.GetCustomAttribute<ScreenAttribute>();
                var parentName = attribute.ParentName;
                var displayName = attribute.DisplayName;
                var icon = attribute.Icon;
                var screenName = attribute.ScreenName;

                var screenObj = ContainerLocator.Container.Resolve(t);
                if(screenObj is BaseScreenViewModel vm)
                {
                    vm.ScreenName = screenName;
                }

                if (!config.ContainsScreen(screenName))
                {
                    config.InactiveScreens.Add(new ScreenInfo
                    {
                        ScreenName = screenName,
                        DisplayName = displayName,
                        Icon = icon,
                        ParentScreenName  = parentName
                    });
                }
            }
        }
    }
}

