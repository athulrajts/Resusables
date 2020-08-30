using Application.UI.AdvancedSetup.Views;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using Prism.Unity;
using System;
using System.Linq;
using System.Reflection;
using Unity;
using Module = KEI.Infrastructure.Prism.Module;

namespace Application.UI
{
    public class ApplicationUIModule : Module
    {
        public ApplicationUIModule(IRegionManager regionManager, IEventAggregator eventAggregator)
            : base(regionManager, eventAggregator)
        {
        }

        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterAdvancedSetup();
            
            base.RegisterTypes(containerRegistry);
        }
    }

    public class AdvancedSetupAttribute : Attribute { }

    public static class ContainerRegistryExtensions
    {
        /// <summary>
        /// Registers all views with attribute <see cref="AdvancedSetupAttribute"/>/>
        /// on to <see cref="RegionNames.AdvancedSetup"/> region.
        /// </summary>
        /// <param name="registry"></param>
        /// <returns></returns>
        public static IContainerRegistry RegisterAdvancedSetup(this IContainerRegistry registry)
        {
            IRegionManager regionManager = registry.GetContainer().Resolve<IRegionManager>();

            var types = Assembly.GetCallingAssembly().ExportedTypes
                .Where(x => x.GetCustomAttribute<AdvancedSetupAttribute>() is { })
                .OrderBy(x => x.Name);

            foreach (var setupType in types)
            {
                regionManager.RegisterViewWithRegion(RegionNames.AdvancedSetup, setupType);
            }

            return registry;
        }
    }
}
