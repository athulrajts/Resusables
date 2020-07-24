using KEI.Infrastructure.Prism;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Application.Engineering
{
    [ModuleDependency("ApplicationModule")]
    public class EngineeringModule : Module
    {
        public EngineeringModule(IRegionManager regionManager, IEventAggregator eventAggregator) : base(regionManager, eventAggregator) { }

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
