using KEI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace KEI.UI.Wpf.Controls.PropertyGridEditors
{
    /// <summary>
    /// Default type editor for <see cref="Infrastructure.DataObject"/> of type file
    /// </summary>
    public class FileNameEditor : TypeEditor<BrowseTextBox>
    {
        protected override void SetValueDependencyProperty()
        {
            ValueProperty = TextBox.TextProperty;
        }

        protected override void SetControlProperties(PropertyItem propertyItem)
        {
            Editor.Height = 23;
            Editor.BrowseButtonWidth = 35;
            Editor.Type = BrowseType.File;
            Editor.VerticalContentAlignment = VerticalAlignment.Center;

            if (propertyItem.PropertyDescriptor is DataObjectPropertyDescriptor desc
                && desc.DataObject is IFileProperty ifp)
            {
                foreach (var filter in ifp.Filters)
                {
                    Editor.Filters.Add(filter);
                }
            }

            base.SetControlProperties(propertyItem);
        }
    }
}
