using KEI.Infrastructure;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.ViewModels
{
    public class TranslationFileEditorTabsViewModel : BindableBase
    {
        private TranslationFileEditorViewModel selectedTab;
        public TranslationFileEditorViewModel SelectedTab
        {
            get { return selectedTab; }
            set { SetProperty(ref selectedTab, value); }
        }
        public ObservableCollection<TranslationFileEditorViewModel> Tabs { get; set; } = new ObservableCollection<TranslationFileEditorViewModel>();

        private readonly IEventAggregator _eventAggregator;
        private readonly IViewService _viewService;
        public TranslationFileEditorTabsViewModel(IEventAggregator eventAggregator, IViewService viewService)
        {
            _eventAggregator = eventAggregator;
            _viewService = viewService;

            eventAggregator.GetEvent<ViewTranslationFileEvent>().Subscribe(x => 
            {
                if (Tabs.FirstOrDefault(y => y.File.FilePath == x.FilePath) is TranslationFileEditorViewModel tb)
                {
                    SelectedTab = tb;
                }
                else
                {
                    var tab = new TranslationFileEditorViewModel(_eventAggregator, _viewService) { File = x };
                    Tabs.Add(tab);
                    SelectedTab = tab;
                }
            });
        }

        private DelegateCommand<TranslationFileEditorViewModel> closeTabCommand;
        public DelegateCommand<TranslationFileEditorViewModel> CloseTabCommand =>
            closeTabCommand ?? (closeTabCommand = new DelegateCommand<TranslationFileEditorViewModel>(ExecuteCloseTabCommand));

        void ExecuteCloseTabCommand(TranslationFileEditorViewModel parameter)
        {
            Tabs.Remove(parameter);
        }
    }
}
