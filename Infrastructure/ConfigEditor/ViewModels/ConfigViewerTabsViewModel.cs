using ConfigEditor.Events;
using KEI.Infrastructure;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ConfigEditor.ViewModels
{
    public class ConfigViewerTabsViewModel : BindableBase
    {
        public ObservableCollection<object> Tabs { get; set; } = new ObservableCollection<object>();

        private object selectedTab;
        public object SelectedTab
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

                if (Tabs.Where(x => x is ConfigViewerViewModel)
                        .FirstOrDefault(x => (x as ConfigViewerViewModel).FullName == dc.Item1) is ConfigViewerViewModel tab)
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

            eventAggregator.GetEvent<ConfigCompareRequest>().Subscribe(c =>
            {
                (string left, string right) = c;

                if (string.IsNullOrEmpty(left) == false && string.IsNullOrEmpty(right) == false)
                {
                    var newTab = new MergeViewModel(_viewService, left, right);

                    Tabs.Add(newTab);

                    SelectedTab = newTab;
                }

            });
        }

        private DelegateCommand<object> closeTabCommand;
        public DelegateCommand<object> CloseTabCommand
            => closeTabCommand ??= new DelegateCommand<object>(ExecuteCloseTabCommand);

        void ExecuteCloseTabCommand(object parameter)
        {
            Tabs.Remove(parameter);
        }
    }
}
