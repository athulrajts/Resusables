using KEI.Infrastructure;
using System;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using Selector = System.Windows.Controls.Primitives.Selector;

namespace KEI.UI.Wpf.Controls.PropertyGridEditors
{
    public class ComboBoxEditor : TypeEditor<VirtualizingComboBox>
    {
        protected override void SetValueDependencyProperty()
        {
            ValueProperty = Selector.SelectedItemProperty;
        }

        protected override void SetControlProperties(PropertyItem propertyItem)
        {
            Editor.IsTextSearchEnabled = true;
            Editor.IsTextSearchCaseSensitive = false;

            if(propertyItem.PropertyDescriptor is DataObjectPropertyDescriptor dop)
            {
                if(dop.DataObject.GetValue() is Enum e)
                {
                    Editor.ItemsSource = Enum.GetValues(e.GetType());
                }
            }
            else
            {
                Editor.ItemsSource = Enum.GetValues(propertyItem.PropertyDescriptor.PropertyType);
            }
        }
    }
}
