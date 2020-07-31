using System.Windows;
using ServiceEditor.ViewModels;

namespace ServiceEditor.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(ServiceEditorViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
