using System.Windows;
using System.Windows.Controls;

namespace KEI.UI.Wpf.Controls
{
    public class NavigationButton : RadioButton
    {
        public Infrastructure.Screen.Icon Icon
        {
            get { return (Infrastructure.Screen.Icon)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Infrastructure.Screen.Icon), typeof(NavigationButton), new PropertyMetadata(Infrastructure.Screen.Icon.None16x));

    }
}
