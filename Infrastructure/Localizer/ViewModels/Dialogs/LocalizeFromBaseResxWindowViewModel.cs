using Localizer.Core;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Localizer.ViewModels
{
    public class LocalizeFromBaseResxWindowViewModel : BindableBase
    {
        private readonly ILocalizerViewSerivce _viewService;
        private readonly Translator _translator;
        public LocalizeFromBaseResxWindowViewModel(ILocalizerViewSerivce viewSerivce)
        {
            _viewService = viewSerivce;
            _translator = new Translator();
        }

        private string baseResXPath;
        public string BaseResXPath
        {
            get { return baseResXPath; }
            set { SetProperty(ref baseResXPath, value); }
        }

        private string lang;
        public string Lang
        {
            get { return lang; }
            set { SetProperty(ref lang, value); }
        }

        private string baseLang = "English";
        public string BaseLang
        {
            get { return baseLang; }
            set { SetProperty(ref baseLang, value); }
        }


        private ResXLocalizationFile resourceFile;
        public ResXLocalizationFile ResourceFile
        {
            get { return resourceFile; }
            set { SetProperty(ref resourceFile, value); }
        }

        private ResXLocalizationFile sourceResX;
        public ResXLocalizationFile SourceResX
        {
            get { return sourceResX; }
            set { SetProperty(ref sourceResX, value); }
        }

        private DelegateCommand generateResXCommand;
        public DelegateCommand GenerateResXCommand 
            => generateResXCommand ??= generateResXCommand = new DelegateCommand(ExecuteGenerateResXCommand);

        void ExecuteGenerateResXCommand()
        {
            if (string.IsNullOrEmpty(BaseResXPath) ||
                File.Exists(BaseResXPath) == false)
            {
                return;
            }

            var langCode = Path.GetFileNameWithoutExtension(BaseResXPath).Split('-').Last();


            BaseLang = Translator.Language.Keys.FirstOrDefault(x => Translator.Language[x] == langCode);

            SourceResX = new ResXLocalizationFile(BaseResXPath, langCode);

            ResourceFile = new ResXLocalizationFile(string.Empty, Translator.Language[Lang]);

            foreach (var translation in sourceResX)
            {
                ResourceFile.AddTranslation(new Translation 
                {
                    Key = translation.Key,
                    EnglishText = translation.TranslatedText,
                });
            }
        }

        private DelegateCommand translateCommand;
        public DelegateCommand TranslateCommand =>
            translateCommand ??= translateCommand = new DelegateCommand(ExecuteTranslateCommand);

        async void ExecuteTranslateCommand()
        {
            int count = 0;
            int total = ResourceFile.Resources.Where(x => string.IsNullOrEmpty(x.TranslatedText)).Count();

            _viewService.SetBusy("Translating");

            foreach (var item in ResourceFile.Resources.Where(x => string.IsNullOrEmpty(x.TranslatedText)))
            {
                _viewService.UpdateBusyText(new[] { "Translating", $"({count++}/{total})" });

                item.TranslatedText = await Task.Run(() => _translator.Translate(item.EnglishText, BaseLang, ResourceFile.Lang));

                await Task.Delay(2000);
            }

            _viewService.SetAvailable();
        }
    }
}
