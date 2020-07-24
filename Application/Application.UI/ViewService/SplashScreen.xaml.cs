using Application.Core;
using KEI.Infrastructure;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace Application.UI
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class SplashScreen : Window, INotifyPropertyChanged
    {
        public SplashScreen(ApplicationMode mode)
        {
            InitializeComponent();
            DataContext = this;
            Mode = mode.ToString();
            SplashScreenLogger.Instance.OnNewMessage += OnNewLogMessage;
        }

        private void OnNewLogMessage(object sender, string e)
        {
            LogMessage = e;
            System.Windows.Forms.Application.DoEvents();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        private string logMessage = "Initializing";
        public string LogMessage
        {
            get { return logMessage; }
            set { logMessage = value; RaisePropertyChanged(nameof(LogMessage)); }
        }

        public string CurrentCulture { get; set; } = CultureInfo.DefaultThreadCurrentCulture.NativeName;

        private string mode;
        public string Mode
        {
            get { return mode; }
            set { mode = value; RaisePropertyChanged(nameof(Mode)); }
        }

    }
}
