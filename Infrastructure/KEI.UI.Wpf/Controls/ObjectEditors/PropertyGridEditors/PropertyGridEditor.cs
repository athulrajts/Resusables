using KEI.UI.Wpf.AttachedProperties;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace KEI.UI.Wpf.Controls.PropertyGridEditors
{
    public class PropertyGridEditor : ITypeEditor
    {
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            Grid editor = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Height = 23
            };

            editor.ColumnDefinitions.Add(new ColumnDefinition());
            editor.ColumnDefinitions.Add(new ColumnDefinition());

            TextBlock text = new TextBlock 
            {
                Text = "(IDataContainer)",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(3, 0, 0 ,0)
            };
            Grid.SetColumn(text, 0);

            PropertyGridLauncherButton button = new PropertyGridLauncherButton
            {
                Width = 35,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };

            var binding = new Binding("Value")
            {
                Source = propertyItem,
                ValidatesOnDataErrors = true,
                ValidatesOnExceptions = true,
                Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay
            };
            
            Grid.SetColumn(button, 1);
            BindingOperations.SetBinding(button, PropertyGridLauncherButton.SelectedObjectProperty, binding);

            editor.Children.Add(text);
            editor.Children.Add(button);

            return editor;
        }


        class PropertyGridLauncherButton : Button
        {

            public object SelectedObject
            {
                get { return GetValue(SelectedObjectProperty); }
                set { SetValue(SelectedObjectProperty, value); }
            }

            public static readonly DependencyProperty SelectedObjectProperty =
                DependencyProperty.Register("SelectedObject", typeof(object), typeof(PropertyGridLauncherButton), new PropertyMetadata(null));


            public PropertyGridLauncherButton()
            {
                Click += PropertyGridButton_Click;
                IconManager.SetIcon(this, Infrastructure.Screen.Icon.Ellipsis16x);
            }

            private void PropertyGridButton_Click(object sender, RoutedEventArgs e)
            {
                var grid = new PropertyGrid();
                var binding = new Binding("SelectedObject") { Source = this, Mode = BindingMode.TwoWay };
                BindingOperations.SetBinding(grid, PropertyGrid.SelectedObjectProperty, binding);

                var window = new Window
                {
                    Height = 450,
                    Width = 450,
                    Content = grid
                };

                window.ShowDialog();
            }
        }
    }
}
