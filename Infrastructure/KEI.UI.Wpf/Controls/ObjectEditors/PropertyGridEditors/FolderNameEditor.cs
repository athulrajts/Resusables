using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace KEI.UI.Wpf.Controls.PropertyGridEditors
{
    /// <summary>
    /// Default type editor for <see cref="Infrastructure.DataObject"/> of type folder
    /// </summary>
    public class FolderNameEditor : TypeEditor<BrowseTextBox>
    {
        protected override void SetValueDependencyProperty()
        {
            ValueProperty = TextBox.TextProperty;
        }

        protected override void SetControlProperties(PropertyItem propertyItem)
        {
            Editor.Height = 23;
            Editor.BrowseButtonWidth = 35;
            Editor.Type = BrowseType.Folder;
            Editor.VerticalContentAlignment = VerticalAlignment.Center;
        }

    }
}
