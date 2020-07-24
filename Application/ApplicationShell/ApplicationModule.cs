using Prism.Ioc;
using Prism.Regions;
using Prism.Modularity;
using KEI.Infrastructure.Prism;
using Prism.Events;

namespace ApplicationShell
{
    [Module(ModuleName = "ApplicationModule")]
    public class ApplicationModule : Module
    {
        public ApplicationModule(IRegionManager regionManager, IEventAggregator eventAggregator) : base(regionManager, eventAggregator) { }

        public override void OnInitialized(IContainerProvider containerProvider)
        {

            base.OnInitialized(containerProvider);
        }

        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterTypes(containerRegistry);
        }

    }
}
