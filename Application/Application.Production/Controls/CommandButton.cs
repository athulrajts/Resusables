using KEI.Infrastructure.Screen;
using System.Windows;
using System.Windows.Controls;

namespace Application.Production.Controls
{
    public class CommandButton : Button
    {
        public Icon Icon
        {
            get { return (Icon)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Icon), typeof(CommandButton), new PropertyMetadata(Icon.None16x));

        static CommandButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CommandButton), new FrameworkPropertyMetadata(typeof(CommandButton)));
        }
    }
}
