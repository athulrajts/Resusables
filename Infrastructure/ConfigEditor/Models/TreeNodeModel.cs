using KEI.Infrastructure.Configuration;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace ConfigEditor.Models
{
    public class TreeNodeModel : BindableBase
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private ObservableCollection<TreeNodeModel> children = new ObservableCollection<TreeNodeModel>();
        public ObservableCollection<TreeNodeModel> Children
        {
            get { return children; }
            set { SetProperty(ref children, value); }
        }

        private bool isExpanded = true;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set { SetProperty(ref isExpanded, value); }
        }

        private ObservableCollection<PropertyObject> dataItems = new ObservableCollection<PropertyObject>();
        public ObservableCollection<PropertyObject> DataItems
        {
            get { return dataItems; }
            set { SetProperty(ref dataItems, value); }
        }

        public TreeNodeModel(IPropertyContainer dc)
        {
            Name = string.IsNullOrEmpty(dc.Name) ? "Untitled" : dc.Name;

            foreach (var item in dc)
            {
                if(item.Value is IPropertyContainer dcValue)
                {
                    Children.Add(new TreeNodeModel(dcValue));
                }
                else
                {
                    DataItems.Add((PropertyObject)item);
                }
            }
        }
    }
}
