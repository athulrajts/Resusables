using KEI.Infrastructure;
using Localizer.Core;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Localizer.ViewModels
{
    public class TranslationFileEditorViewModel : BindableBase
    {
        private CancellationTokenSource _cancellationTokenSource;
        private const int DELAY = 2000;

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
            translateCommand ??= translateCommand = new DelegateCommand(ExecuteTranslateCommand);

        private async void ExecuteTranslateCommand()
        {
            var translations = File.Resources.Where(x => string.IsNullOrEmpty(x.TranslatedText));
            int total = translations.Count();
            _cancellationTokenSource = new CancellationTokenSource();

            _viewService.SetBusy("Translating");

            var progress = new Progress<int>(count => _viewService.UpdateBusyText("Translating", $"{count}/{total}"));

            await Translate(translations, progress, _cancellationTokenSource.Token);

            _viewService.SetAvailable();
        }

        private DelegateCommand<IList> translateSelected;
        public DelegateCommand<IList> TranslateSelected =>
            translateSelected ??= translateSelected = new DelegateCommand<IList>(ExecuteTranslateSelected);

        private async void ExecuteTranslateSelected(IList param)
        {
            if (param is IEnumerable<Translation> translations)
            {
                int total = translations.Count();

                _cancellationTokenSource = new CancellationTokenSource();

                _viewService.SetBusy("Translating");

                var progress = new Progress<int>(count => _viewService.UpdateBusyText("Translating", $"{count}/{total}"));

                await Translate(translations, progress, _cancellationTokenSource.Token);

                _viewService.SetAvailable();

            }

        }

        private DelegateCommand translateAll;
        public DelegateCommand TranslateAll =>
            translateAll ??= translateAll = new DelegateCommand(ExecuteTranslateAll);

        private async void ExecuteTranslateAll()
        {
            int total = File.Resources.Count;
            _cancellationTokenSource = new CancellationTokenSource();

            var progress = new Progress<int>(count => _viewService.UpdateBusyText("Translating", $"{count}/{total}"));

            _viewService.SetBusy("Translating");

            await Translate(File.Resources, progress, _cancellationTokenSource.Token);

            _viewService.SetAvailable();
        }

        private async Task Translate(IEnumerable<Translation> items, IProgress<int> progress, CancellationToken cancellationToken)
        {
            int current = 0;
            foreach (var item in items)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                item.TranslatedText = await Task.Run(() => Translator.Translate(item.EnglishText, "English", File.Lang));

                progress.Report(++current);

                await Task.Delay(DELAY);
            }
        }

        private DelegateCommand writeCommand;
        public DelegateCommand WriteCommand =>
            writeCommand ??= writeCommand = new DelegateCommand(ExecuteWriteCommand);

        private void ExecuteWriteCommand()
        {
            File?.Write();
        }
    }
}
