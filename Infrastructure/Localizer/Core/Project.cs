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
        public ObservableCollection<ResXLocalizationFile> TranslationFiles { get; set; } = new ObservableCollection<ResXLocalizationFile>();

        private List<string> stringKeys = new List<string>();

        public Project(string resourceDirectory)
        {
            ResourceDirectory = resourceDirectory;

            var keyFileName = $@"..\..\Localization\{ProjectName}_Resources-en.txt";

            if (File.Exists(keyFileName))
            {
                foreach (var item in File.ReadAllLines(keyFileName))
                {
                    stringKeys.Add(item?.Trim());
                }
            }

            foreach (var file in Directory.GetFiles(ResourceDirectory, "*.resx"))
            {
                if (Path.GetFileName(file) != "Resources.resx")
                {
                    var split = Path.GetFileNameWithoutExtension(file).Split('-');
                    if (split.Length == 2)
                    {
                        var resxFile = new ResXLocalizationFile(file, split[1]);
                        stringKeys.ForEach(key => resxFile.AddTranslation(new Translation { Key = key, EnglishText = key.Replace("_", " ").ToLower()}));
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

                var file = new ResXLocalizationFile($@"{ResourceDirectory}\Resources-{twoLetterCode}.resx", twoLetterCode);

                stringKeys.ForEach(key => file.AddTranslation(new Translation {Key = key, EnglishText = key.Replace("_", " ").ToLower() }));

                TranslationFiles.Add(file);
            }

        }
    }
}
