using System.Collections.ObjectModel;
using Prism.Mvvm;
using KEI.Infrastructure;

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

            foreach (PropertyObject item in dc)
            {
                var value = item.GetValue();

                if(value is null)
                {
                    item.SetBrowsePermission(BrowseOptions.NonBrowsable);
                }
                else if(value is IPropertyContainer dcValue)
                {
                    item.SetBrowsePermission(BrowseOptions.NonBrowsable);
                    Children.Add(new TreeNodeModel(dcValue));
                }
                else
                {
                    item.SetBrowsePermission(BrowseOptions.Browsable);
                }
            }
        }
    }
}
