using KEI.UI.Wpf.Controls.Icon;
using System.Windows;
using System.Windows.Controls;

namespace KEI.UI.Wpf.Controls
{
    public class IconPicker : Button
    {
        public Infrastructure.Screen.Icon Icon
        {
            get { return (Infrastructure.Screen.Icon)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Infrastructure.Screen.Icon), typeof(IconPicker), new PropertyMetadata(Infrastructure.Screen.Icon.None16x));

        public IconPicker()
        {
            Click += IconPicker_Click;
        }

        private void IconPicker_Click(object sender, RoutedEventArgs e)
        {
            var picker = new IconViewer();

            if(picker.ShowDialog() == true)
                Icon = picker.SelectedIcon;
        }
    }
}
