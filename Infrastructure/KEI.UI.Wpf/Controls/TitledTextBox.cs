using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KEI.UI.Wpf.Controls
{
    public class TitledTextBox : TextBox
    {
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(TitledTextBox), new PropertyMetadata(string.Empty));

        public double TitleSize
        {
            get { return (double)GetValue(TitleSizeProperty); }
            set { SetValue(TitleSizeProperty, value); }
        }

        public static readonly DependencyProperty TitleSizeProperty =
            DependencyProperty.Register("TitleSize", typeof(double), typeof(TitledTextBox), new PropertyMetadata(11.0));

        public Brush TitleForeground
        {
            get { return (Brush)GetValue(TitleForegroundProperty); }
            set { SetValue(TitleForegroundProperty, value); }
        }

        public static readonly DependencyProperty TitleForegroundProperty =
            DependencyProperty.Register("TitleForeground", typeof(Brush), typeof(TitledTextBox), new PropertyMetadata(Brushes.Gray));

        public FontWeight TitleWeight
        {
            get { return (FontWeight)GetValue(TitleWeightProperty); }
            set { SetValue(TitleWeightProperty, value); }
        }

        public static readonly DependencyProperty TitleWeightProperty =
            DependencyProperty.Register("TitleWeight", typeof(FontWeight), typeof(TitledTextBox), new PropertyMetadata(FontWeights.DemiBold));

    }
}
