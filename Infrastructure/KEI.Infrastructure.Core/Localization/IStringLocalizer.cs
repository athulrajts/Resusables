using Prism.Events;

namespace KEI.Infrastructure.Localizer
{
    public interface IStringLocalizer
    {
        string GetLocalizedString(string name);
        string GetLocalizedString(string name, params object[] args);
        string this[string name] { get; }
        string this[string name, params object[] args] { get; }
        string Name { get; set; }
        void Reload(string twoLetterISOLanguageName);
    }

    public class LanguageChangedEvent : PubSubEvent<string> { }
}
