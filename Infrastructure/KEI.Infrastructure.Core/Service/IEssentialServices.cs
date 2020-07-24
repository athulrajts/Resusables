using Prism.Events;

namespace KEI.Infrastructure.Service
{
    public interface IEssentialServices
    {
        IEventAggregator EventAggregator { get; }
        IViewService ViewService { get; }
        ILogManager LogManager { get; }
    }

    /// <summary>
    /// Most Viewmodels requires <see cref="IEventAggregator"/>, <see cref="IViewService"/> and <see cref="ILogManager"/>
    /// Made class containing all of these, so that the constructor of consuming classes can be cleaner
    /// </summary>
    public class EssentialServices : IEssentialServices
    {
        public IEventAggregator EventAggregator { get; }
        public IViewService ViewService { get; }
        public ILogManager LogManager { get; }

        public EssentialServices(IEventAggregator ea, IViewService vs, ILogManager lm)
        {
            EventAggregator = ea;
            ViewService = vs;
            LogManager = lm;
        }
    }
}
