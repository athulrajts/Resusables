using System.Windows;
using System.Windows.Controls;
using KEI.Infrastructure.Screen;

namespace KEI.UI.Wpf.AttachedProperties
{
    /// <summary>
    /// Attached property to Set <see cref="ContentControl.Content"/> property
    /// base on <see cref="Icon"/> from Reource dictionary defined
    /// in KEI.Icons Project
    /// </summary>
    public class IconManager
    {
        public static Icon GetIcon(DependencyObject obj)
        {
            return (Icon)obj.GetValue(IconProperty);
        }

        public static void SetIcon(DependencyObject obj, Icon value)
        {
            obj.SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.RegisterAttached("Icon", typeof(Icon), typeof(IconManager), new PropertyMetadata(Icon.None16x, OnIconChanged));

        private static readonly ResourceDictionary icons = new ResourceDictionary
        {
            Source = new System.Uri("pack://application:,,,/KEI.Icons;component/VisualStudioIcons/Icons.xaml", System.UriKind.Absolute)
        };

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is ContentControl contentControl && e.NewValue != null)
            {
                contentControl.Content = icons[e.NewValue.ToString()];
            }
        }
    }
}
