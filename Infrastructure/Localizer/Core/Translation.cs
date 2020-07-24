using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.Core
{
    public class Translation : BindableBase
    {
        public string Key { get; set; }
        public string EnglishText => Key.Replace("_", " ");

        private string translatedText;
        public string TranslatedText
        {
            get { return translatedText; }
            set { SetProperty(ref translatedText, value); }
        }
    }
}
