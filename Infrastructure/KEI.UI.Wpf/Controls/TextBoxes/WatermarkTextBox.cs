using System.Windows;
using System.Windows.Controls;

namespace KEI.UI.Wpf.Controls
{
    public class WatermarkTextBox : TextBox
    {
        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(string), typeof(WatermarkTextBox), new PropertyMetadata("Placeholder"));


        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(WatermarkTextBox), 
                new PropertyMetadata(new CornerRadius(0)));



        public HorizontalAlignment WatermarkHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(WatermarkHorizontalAlignmentProperty); }
            set { SetValue(WatermarkHorizontalAlignmentProperty, value); }
        }

        public static readonly DependencyProperty WatermarkHorizontalAlignmentProperty =
            DependencyProperty.Register("WatermarkHorizontalAlignment", typeof(HorizontalAlignment),
                typeof(WatermarkTextBox), new PropertyMetadata(HorizontalAlignment.Left));


        public VerticalAlignment WatermarkVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(WatermarkVerticalAlignmentProperty); }
            set { SetValue(WatermarkVerticalAlignmentProperty, value); }
        }

        public static readonly DependencyProperty WatermarkVerticalAlignmentProperty =
            DependencyProperty.Register("WatermarkVerticalAlignment", typeof(VerticalAlignment), typeof(WatermarkTextBox),
                new PropertyMetadata(VerticalAlignment.Center));




        static WatermarkTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WatermarkTextBox), new FrameworkPropertyMetadata(typeof(WatermarkTextBox)));
        }

    }
}
