using Prism.Events;
using KEI.Infrastructure.Configuration;

namespace KEI.Infrastructure.Events
{
    public class PropertyChangedPayLoad<T>
    {
        public T NewValue { get; set; }
        public T OldValue { get; set; }

        public PropertyChangedPayLoad(T oldVal, T newVal)
        {
            OldValue = oldVal;
            NewValue = newVal;
        }
    }

    /// <summary>
    /// Event Fired When Application Mode is changed.
    /// </summary>
    public class SetAvailableEvent : PubSubEvent { }
    public class RecipeLoadedEvent : PubSubEvent<IPropertyContainer> { }
}
