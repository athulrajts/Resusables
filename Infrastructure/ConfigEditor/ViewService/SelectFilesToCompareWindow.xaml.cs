using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ConfigEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for SelectFilesToCompareWindow.xaml
    /// </summary>
    public partial class SelectFilesToCompareWindow : Window, INotifyPropertyChanged
    {
        #region INPC
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string property = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        
        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string property = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value) == true)
            {
                return false;
            }

            storage = value;

            RaisePropertyChanged(property);

            return true;
        }

        #endregion

        public SelectFilesToCompareWindow()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
        }

        private string left;
        public string LeftFile
        {
            get { return left; }
            set { SetProperty(ref left, value); }
        }

        private string right;
        public string RightFile
        {
            get { return right; }
            set { SetProperty(ref right, value); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = !(string.IsNullOrEmpty(LeftFile) || string.IsNullOrEmpty(RightFile));
        }
    }
}
