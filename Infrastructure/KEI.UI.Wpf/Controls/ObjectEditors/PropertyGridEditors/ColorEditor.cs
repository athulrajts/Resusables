﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace KEI.UI.Wpf.Controls.PropertyGridEditors
{
    /// <summary>
    /// Default type edit for <see cref="Infrastructure.DataObject"/> of type color
    /// </summary>
    public class ColorEditor : TypeEditor<ColorPicker>
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

        class ColorConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if(value is Infrastructure.Color c)
                {
                    return System.Windows.Media.Color.FromRgb(c.R, c.G, c.B);
                }

                return DependencyProperty.UnsetValue;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if(value is System.Windows.Media.Color c)
                {
                    return new Infrastructure.Color(c.R, c.G, c.B);
                }

                return DependencyProperty.UnsetValue;
            }
        }
    }
}
