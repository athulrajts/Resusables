using Prism.Mvvm;

namespace Localizer.Core
{
    public class Translation : BindableBase
    {
        public string Key { get; set; }
        public string EnglishText { get; set; }

        private string translatedText;
        public string TranslatedText
        {
            get { return translatedText; }
            set { SetProperty(ref translatedText, value); }
        }
    }

}
