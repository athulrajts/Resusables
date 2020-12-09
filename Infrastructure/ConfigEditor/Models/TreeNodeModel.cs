using KEI.Infrastructure;
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

        public IDataContainer Container { get; set; }

        public TreeNodeModel(IPropertyContainer dc)
        {
            Name = string.IsNullOrEmpty(dc.Name) ? "Untitled" : dc.Name;

            Container = dc;

            foreach (var item in dc)
            {
                if(item.GetValue() is IPropertyContainer dcValue)
                {
                    if(item is PropertyObject p)
                    {
                        p.SetBrowsePermission(BrowseOptions.NonBrowsable);
                    }

                    Children.Add(new TreeNodeModel(dcValue));
                }
                else
                {
                    var data = (PropertyObject)item;
                    data.SetBrowsePermission(BrowseOptions.Browsable);
                }
            }
        }
    }
}
