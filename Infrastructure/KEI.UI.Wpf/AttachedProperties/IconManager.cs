using KEI.Infrastructure.Screen;
using System.Windows;
using System.Windows.Controls;

namespace KEI.UI.Wpf.AttachedProperties
{
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

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is ContentControl contentControl && e.NewValue != null)
            {
                contentControl.Content = Application.Current.TryFindResource(e.NewValue.ToString());
            }
        }
    }
}
