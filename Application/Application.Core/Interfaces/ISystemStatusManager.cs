using KEI.Infrastructure.Events;
using Prism.Events;
using System.ComponentModel;

namespace Application.Core
{
    public interface ISystemStatusManager
    {
        ApplicationMode ApplicationMode { get; set; }
        string CurrentScreen { get; set; }
        Language CurrentLanguage { get; set; }
    }

    public enum ApplicationMode
    {
        Production,
        Engineering
    }

    public enum Language
    {
        [Description("en")]
        English,

        [Description("ja")]
        Japanese,

        [Description("ko")]
        Korean,

        [Description("vi")]
        Vietnamise,
    }

    public class ApplicationModeChangedEvent : PubSubEvent<PropertyChangedPayLoad<ApplicationMode>> { }
}
