using ConfigEditor.Events;
using ConfigEditor.Models;
using KEI.Infrastructure;
using KEI.Infrastructure.Configuration;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;

namespace ConfigEditor
{
    public class ConfigEditorViewModel : BindableBase
    {
        private readonly IViewService _viewService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private IPropertyContainer openedDataContainer;
        public ConfigEditorViewModel(IViewService viewService, IEventAggregator ea, IRegionManager regionManager)
        {
            _viewService = viewService;
            _eventAggregator = ea;
            _regionManager = regionManager;
        }


        private DelegateCommand openFileCommand;
        public DelegateCommand OpenFileCommand =>
            openFileCommand ?? (openFileCommand = new DelegateCommand(ExecuteOpenFileCommand));

        void ExecuteOpenFileCommand()
        {
            var fileName = _viewService.BrowseFile("DataContainer Files", "rcp,xcfg");

            if (string.IsNullOrEmpty(fileName))
                return;

            OpenFile(fileName);
        }

        public void OpenFile(string path)
        {
            var dc = PropertyContainerBuilder.FromFile(path) ?? DataContainer.FromFile(path).ToPropertyContainer();

            if (dc == null)
            {
                _viewService.Error("Invalid File !");
                return;
            }

            openedDataContainer = dc;

            _eventAggregator.GetEvent<DataContainerOpened>().Publish(new Tuple<string, IPropertyContainer>(path, dc));

            NavigateCommand.Execute(nameof(Views.ConfigViewerTabs));
        }

        private DelegateCommand<string> navigateCommand;
        public DelegateCommand<string> NavigateCommand =>
            navigateCommand ?? (navigateCommand = new DelegateCommand<string>(ExecuteNavigateCommand));

        void ExecuteNavigateCommand(string parameter)
        {
            _regionManager.RequestNavigate("MainContent", parameter);
        }
    }
}
