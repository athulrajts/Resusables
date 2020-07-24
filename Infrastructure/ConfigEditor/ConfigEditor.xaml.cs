using KEI.Infrastructure.Configuration;
using System.Windows;

namespace ConfigEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ConfigEditorWindow : Window
    {
        public ConfigEditorWindow(ConfigEditorViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
