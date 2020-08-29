using KEI.Infrastructure.Prism;
using Prism.Events;
using Prism.Regions;

namespace Application.UI
{
    public class ApplicationUIModule : Module
    {
        public ApplicationUIModule(IRegionManager regionManager, IEventAggregator eventAggregator)
            : base(regionManager, eventAggregator)
        {
        }
    }
}
