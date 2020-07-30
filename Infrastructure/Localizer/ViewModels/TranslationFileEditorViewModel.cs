using KEI.Infrastructure;
using Localizer.Core;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Localizer.ViewModels
{
    public class TranslationFileEditorViewModel : BindableBase
    {
        private ResXLocalizationFile file;
        public ResXLocalizationFile File
        {
            get { return file; }
            set { SetProperty(ref file, value); }
        }

        private Translator Translator = new Translator();
        private readonly ILocalizerViewSerivce _viewService;
        public TranslationFileEditorViewModel(IEventAggregator eventAggregator, ILocalizerViewSerivce viewService)
        {
            _viewService = viewService;
        }


        private DelegateCommand translateCommand;
        public DelegateCommand TranslateCommand =>
            translateCommand ?? (translateCommand = new DelegateCommand(ExecuteTranslateCommand));

        async void ExecuteTranslateCommand()
        {
            int count = 0;
            int total = File.Resources.Where(x => string.IsNullOrEmpty(x.TranslatedText)).Count();

            _viewService.SetBusy("Translating");

            foreach (var item in File.Resources.Where(x => string.IsNullOrEmpty(x.TranslatedText)))
            {
                _viewService.UpdateBusyText(new[] { "Translating", $"({count++}/{total})" });

                item.TranslatedText = await Task.Run(() => Translator.Translate(item.EnglishText, "English", File.Lang));

                await Task.Delay(2000);
            }

            _viewService.SetAvailable();
        }

        private DelegateCommand<IList> translateSelected;
        public DelegateCommand<IList> TranslateSelected =>
            translateSelected ?? (translateSelected = new DelegateCommand<IList>(ExecuteTranslateSelected));

        async void ExecuteTranslateSelected(IList param)
        {
            if(param != null)
            {
                int count = 0;
                int total = param.Count;

                _viewService.SetBusy($"Translating");

                foreach (var item in param)
                {
                    if(item is Translation t)
                    {
                        _viewService.UpdateBusyText(new[] {"Translating", $"({count++}/{total})" });

                        t.TranslatedText = await Task.Run(() => Translator.Translate(t.EnglishText, "English", File.Lang));

                        await Task.Delay(2000);
                    }
                }
                _viewService.SetAvailable();
            }
        }

        private DelegateCommand translateAll;
        public DelegateCommand TranslateAll =>
            translateAll ?? (translateAll = new DelegateCommand(ExecuteTranslateAll));

        async void ExecuteTranslateAll()
        {
            int count = 0;
            int total = File.Resources.Count;

            _viewService.SetBusy("Translating");

            foreach (var item in File.Resources)
            {
                _viewService.UpdateBusyText(new[] { "Translating", $"({count++}/{total})" });

                item.TranslatedText = await Task.Run(() => Translator.Translate(item.EnglishText, "English", File.Lang));

                await Task.Delay(2000);
            }

            _viewService.SetAvailable();
        }

        private DelegateCommand writeCommand;
        public DelegateCommand WriteCommand =>
            writeCommand ?? (writeCommand = new DelegateCommand(ExecuteWriteCommand));

        void ExecuteWriteCommand()
        {
            File?.Write();
        }
    }
}
