using System.Windows;
using System.Windows.Controls;
using KEI.Infrastructure.Screen;

namespace Application.Production.Controls
{
    public class NavigationButton : RadioButton
    {
        public Icon Icon
        {
            get { return (Icon)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Icon), typeof(NavigationButton), new PropertyMetadata(Icon.None16x));

        static NavigationButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationButton), new FrameworkPropertyMetadata(typeof(NavigationButton)));
        }

    }
}
