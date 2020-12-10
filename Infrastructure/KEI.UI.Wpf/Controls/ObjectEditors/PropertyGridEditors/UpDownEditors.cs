using KEI.Infrastructure;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace KEI.UI.Wpf.Controls.PropertyGridEditors
{
    public class IntegerUpDownEditor : TypeEditor<PropertyGridEditorIntegerUpDown>
    {
        protected override void SetValueDependencyProperty()
        {
            ValueProperty = IntegerUpDown.ValueProperty;
        }

        protected override void SetControlProperties(PropertyItem propertyItem)
        {
            Editor.TextAlignment = System.Windows.TextAlignment.Left;

            if (propertyItem.PropertyDescriptor is DataObjectPropertyDescriptor dp)
            {
                if(dp.DataObject is INumericPropertyObject ndo)
                {
                    Editor.Increment = (int?)ndo.Increment;
                    Editor.Maximum = (int?)ndo.Max;
                    Editor.Minimum = (int?)ndo.Min;
                }
            }
        }
    }

    public class DoubleUpDownEditor : TypeEditor<PropertyGridEditorDoubleUpDown>
    {
        protected override void SetValueDependencyProperty()
        {
            ValueProperty = DoubleUpDown.ValueProperty;
        }

        protected override void SetControlProperties(PropertyItem propertyItem)
        {
            Editor.TextAlignment = System.Windows.TextAlignment.Left;

            if (propertyItem.PropertyDescriptor is DataObjectPropertyDescriptor dp)
            {
                if (dp.DataObject is INumericPropertyObject ndo)
                {
                    Editor.Increment = (double?)ndo.Increment;
                    Editor.Maximum = (double?)ndo.Max;
                    Editor.Minimum = (double?)ndo.Min;
                }
            }
        }
    }

    public class FloatUpDownEditor : TypeEditor<PropertyGridEditorSingleUpDown>
    {
        protected override void SetValueDependencyProperty()
        {
            ValueProperty = SingleUpDown.ValueProperty;
        }

        protected override void SetControlProperties(PropertyItem propertyItem)
        {
            Editor.TextAlignment = System.Windows.TextAlignment.Left;

            if (propertyItem.PropertyDescriptor is DataObjectPropertyDescriptor dp)
            {
                if (dp.DataObject is INumericPropertyObject ndo)
                {
                    Editor.Increment = (float?)ndo.Increment;
                    Editor.Maximum = (float?)ndo.Max;
                    Editor.Minimum = (float?)ndo.Min;
                }
            }
        }
    }
}
