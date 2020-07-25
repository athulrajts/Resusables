using Localizer.Views.Dialogs;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.Core
{
    public class Project : BindableBase
    {
        public string ResourceDirectory { get; set; }
        public string ProjectName => Path.GetFileName(Path.GetDirectoryName(ResourceDirectory));
        public ObservableCollection<TranslationFile> TranslationFiles { get; set; } = new ObservableCollection<TranslationFile>();

        private List<string> stringKeys = new List<string>();

        public Project(string resourceDirectory)
        {
            ResourceDirectory = resourceDirectory;

            var keyFileName = $@"..\..\Localization\{ProjectName}_Resources-en.resx";

            if (File.Exists(keyFileName))
            {
                using (var rr = new ResXResourceReader(keyFileName))
                {
                    foreach (DictionaryEntry item in rr)
                    {
                        if (item.Value is string)
                        {
                            stringKeys.Add(item.Key.ToString());
                        }
                    }
                } 
            }

            foreach (var file in Directory.GetFiles(ResourceDirectory, "*.resx"))
            {
                if (Path.GetFileName(file) != "Resources.resx")
                {
                    var split = Path.GetFileNameWithoutExtension(file).Split('-');
                    if (split.Length == 2)
                    {
                        var resxFile = new TranslationFile(file, split[1]);
                        stringKeys.ForEach(key => resxFile.AddResource(key, string.Empty));
                        TranslationFiles.Add(resxFile);
                    }
                }
            }
        }

        private DelegateCommand<Project> addTranslation;
        public DelegateCommand<Project> AddTranslationCommand 
            => addTranslation ??= new DelegateCommand<Project>(ExecuteAddTranslationCommand);

        public void ExecuteAddTranslationCommand(Project param)
        {
            var window = new ChooseLanguage();
            if (window.ShowDialog() == true)
            {
                var selected = window.SelectedLanguage;
                var twoLetterCode = Translator.Language[selected];

                if (TranslationFiles.Any(x => x.Lang == twoLetterCode))
                {
                    return;
                }

                var file = new TranslationFile($@"{ResourceDirectory}\Resources-{twoLetterCode}.resx", twoLetterCode);

                stringKeys.ForEach(key => file.AddResource(key, string.Empty));

                TranslationFiles.Add(file);
            }

        }
    }
}
