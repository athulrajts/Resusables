using ConfigEditor.Dialogs;
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

        private readonly IConfigEditorViewService _viewService;
        private readonly IDialogFactory _dialogFactory;

        public ConfigViewerTabsViewModel(IEventAggregator eventAggregator, IConfigEditorViewService viewService, IDialogFactory dialogFactory)
        {
            _viewService = viewService;
            _dialogFactory = dialogFactory;

            eventAggregator.GetEvent<DataContainerOpened>().Subscribe(dc =>
            {
                var newTab = new ConfigViewerViewModel(dc.Item2, dc.Item1, _viewService);
                Tabs.Add(newTab);
                SelectedTab = newTab;
            });

            eventAggregator.GetEvent<ConfigCompareRequest>().Subscribe(c =>
            {
                (string left, string right) = c;

                if (string.IsNullOrEmpty(left) == false && string.IsNullOrEmpty(right) == false)
                {
                    var newTab = new MergeViewModel(_viewService, _dialogFactory, left, right);

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
