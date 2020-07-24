using KEI.Infrastructure.UserManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KEI.UI.Wpf.ViewService
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window, INotifyPropertyChanged
    {
        private IUserManager _userManager;
        public LoginWindow(IUserManager userManager)
        {
            InitializeComponent();
            DataContext = this;
            _userManager = userManager;
            HeaderTitle = "Login";//Assembly.GetEntryAssembly().GetName().Name;
            usernameTxt.Focus();
        }

        private string title;
        public string HeaderTitle
        {
            get => title;
            set
            {
                title = value;
                RaisePropertyChanged(nameof(HeaderTitle));
            }
        }

        private string username;
        public string Username
        {
            get => username;
            set
            {
                username = value;
                RaisePropertyChanged(nameof(Username));
            }
        }

        private string password;
        public string Password
        {
            get => password;
            set
            {
                password = value;
                RaisePropertyChanged(nameof(Password));
            }
        }

        private string errorText;
        public string ErrorText
        {
            get => errorText;
            set
            {
                errorText = value;
                RaisePropertyChanged(nameof(ErrorText));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = (sender as PasswordBox).Password;
        }

        private void OnLogin(object sender, RoutedEventArgs e)
        {
            if (_userManager.ValidateLogin(Username, Password))
                DialogResult = true;
            else
                ErrorText = "Invalid login credentials";
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
