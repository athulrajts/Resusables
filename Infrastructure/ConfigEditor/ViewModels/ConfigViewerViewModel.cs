using ConfigEditor.Events;
using ConfigEditor.Models;
using KEI.Infrastructure;
using KEI.Infrastructure.Configuration;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ConfigEditor.ViewModels
{
    public class ConfigViewerViewModel : BindableBase
    {
        private ObservableCollection<TreeNodeModel> tree;
        public ObservableCollection<TreeNodeModel> Tree
        {
            get { return tree; }
            set { SetProperty(ref tree, value); }
        }

        private TreeNodeModel selectedNode;
        public TreeNodeModel SelectedNode
        {
            get { return selectedNode; }
            set { SetProperty(ref selectedNode, value); }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private string fullName;
        public string FullName
        {
            get { return fullName; }
            set { SetProperty(ref fullName, value); }
        }

        private IPropertyContainer openedDataContainer = null;
        private readonly IViewService _viewService;

        public ConfigViewerViewModel(IPropertyContainer rootNode, string name, IViewService viewService)
        {
            Tree = new ObservableCollection<TreeNodeModel>();
            openedDataContainer = rootNode;
            Tree.Add(new TreeNodeModel(rootNode));
            FullName = name;
            Name = Path.GetFileName(name);
            _viewService = viewService;
            SelectedNode = Tree[0];
        }

        private DelegateCommand saveFileCommand;
        public DelegateCommand SaveFileCommand 
            => saveFileCommand ??= (saveFileCommand = new DelegateCommand(ExecuteSaveFileCommand, () => openedDataContainer is IPropertyContainer));

        void ExecuteSaveFileCommand() => _viewService.SaveFile((path) => openedDataContainer.Store(path));
    }
}
