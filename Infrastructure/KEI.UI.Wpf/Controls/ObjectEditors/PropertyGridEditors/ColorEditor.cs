using KEI.UI.Wpf.Converters;
using System.Windows.Data;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace KEI.UI.Wpf.Controls.PropertyGridEditors
{
    /// <summary>
    /// Default type edit for <see cref="Infrastructure.DataObject"/> of type color
    /// </summary>
    public class ColorEditor : TypeEditor<PropertyGridEditorColorPicker>
    {
        protected override void SetValueDependencyProperty()
        {
            ValueProperty = ColorPicker.SelectedColorProperty;
        }

        protected override void SetControlProperties(PropertyItem propertyItem)
        {
            Editor.DisplayColorAndName = true;
        }

        protected override IValueConverter CreateValueConverter()
        {
            return new ColorConverter();
        }
    }
}
