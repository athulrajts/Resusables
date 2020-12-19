using System.Windows;
using System.Windows.Controls;

namespace KEI.UI.Wpf.Controls
{
    public class VirtualizingComboBox : ComboBox
    {
        static VirtualizingComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VirtualizingComboBox), new FrameworkPropertyMetadata(typeof(VirtualizingComboBox)));
        }
    }
}
