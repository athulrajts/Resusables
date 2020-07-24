using System.Windows;
using System.ComponentModel;
using KEI.UI.Wpf.Hotkey;
using System.Windows.Input;
using ApplicationCommands = ApplicationShell.Commands.ApplicationCommands;

namespace ApplicationShell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        public MainWindow(IHotkeyService hotkeyService, ApplicationCommands commands)
        {
            hotkeyService.SetGestureProvider(InputBindings);

            InitializeComponent();


            StateChanged += (ss, ee) => RaisePropertyChanged(nameof(WindowPadding));

            hotkeyService.AddReadonlyGesture(GestureCache.GetGesture(
                "Quit.Application",
                commands.ExitApplicationCommand,
                null,
                Key.Q,
                ModifierKeys.Control));
        }


        public Thickness WindowPadding => (this.WindowState == WindowState.Maximized) ? new Thickness(7) : new Thickness(0);

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        
    }

}
