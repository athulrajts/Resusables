using ConfigEditor.Events;
using KEI.Infrastructure;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;

namespace ConfigEditor.ViewModels
{
    public class ConfigViewerTabsViewModel : BindableBase
    {
        public ObservableCollection<ConfigViewerViewModel> Tabs { get; set; } = new ObservableCollection<ConfigViewerViewModel>();

        private ConfigViewerViewModel selectedTab;
        public ConfigViewerViewModel SelectedTab
        {
            get { return selectedTab; }
            set { SetProperty(ref selectedTab, value); }
        }

        private readonly IViewService _viewService;
        public ConfigViewerTabsViewModel(IEventAggregator eventAggregator, IViewService viewService)
        {
            _viewService = viewService;
            eventAggregator.GetEvent<DataContainerOpened>().Subscribe(dc => 
            {
                var newTab = new ConfigViewerViewModel(dc.Item2, dc.Item1, _viewService);

                if (Tabs.FirstOrDefault(x => x.FullName == dc.Item1) is ConfigViewerViewModel tab)
                {
                    var index = Tabs.IndexOf(tab);
                    Tabs.Remove(tab);
                    Tabs.Insert(index, newTab);
                }
                else
                {
                    Tabs.Add(newTab);
                }

                SelectedTab = newTab;
            });
        }

        private DelegateCommand<ConfigViewerViewModel> closeTabCommand;
        public DelegateCommand<ConfigViewerViewModel> CloseTabCommand =>
            closeTabCommand ?? (closeTabCommand = new DelegateCommand<ConfigViewerViewModel>(ExecuteCloseTabCommand));

        void ExecuteCloseTabCommand(ConfigViewerViewModel parameter)
        {
            Tabs.Remove(parameter);
        }
    }
}
