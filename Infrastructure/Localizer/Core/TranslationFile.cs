using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Resources;

namespace Localizer.Core
{
    public class TranslationFile : BindableBase
    {
        public string Lang { get; set; }
        public string LangCode { get; set; }
        public string FilePath { get; set; }
        public ObservableCollection<Translation> Resources { get; set; } = new ObservableCollection<Translation>();

        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        public TranslationFile(string path, string language)
        {
            FilePath = path;

            foreach (var item in Translator.Language)
            {
                if (item.Value == language)
                    Lang = item.Key;
            }

            LangCode = language;
            Name = Path.GetFileName(FilePath);


            if (File.Exists(FilePath))
            {
                using (var rr = new ResXResourceReader(FilePath))
                {
                    foreach (DictionaryEntry item in rr)
                    {
                        if (item.Value is string)
                        {
                            Resources.Add(new Translation { Key = item.Key.ToString(), TranslatedText = item.Value.ToString() });
                        }
                    }
                } 
            }
        }

        public IEnumerator<Translation> GetEnumerator() => Resources.GetEnumerator();

        public bool ContainsKey(string key)
        {
            return Resources.FirstOrDefault(x => x.Key == key) is Translation;
        }

        public void Write()
        {
            using (var rw = new ResXResourceWriter(FilePath))
            {
                foreach (var item in Resources)
                {
                    rw.AddResource(item.Key, item.TranslatedText);
                }
            }
        }

        public void AddResource(string key, string value)
        {
            if(!ContainsKey(key))
            {
                Resources.Add(new Translation { Key = key, TranslatedText = value});
            }
        }

    }
}
