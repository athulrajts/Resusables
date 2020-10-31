using System.Windows;
using System.Windows.Controls;
using KEI.UI.Wpf.AttachedProperties;

namespace KEI.UI.Wpf.Controls
{
    /// <summary>
    /// Control to display icons
    /// </summary>
    public class IconControl : ContentControl
    {
        public Infrastructure.Screen.Icon Icon
        {
            get { return (Infrastructure.Screen.Icon)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Infrastructure.Screen.Icon), 
                typeof(IconControl), 
                new PropertyMetadata(Infrastructure.Screen.Icon.None16x, OnIconChanged));

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Infrastructure.Screen.Icon icon)
            {
                IconManager.SetIcon(d, icon); 
            }
        }
    }
}
