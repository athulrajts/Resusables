using Localizer.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

namespace Localizer.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for ChooseLanguage.xaml
    /// </summary>
    public partial class ChooseLanguage : Window, INotifyPropertyChanged
    {
        public ChooseLanguage()
        {
            InitializeComponent();
            DataContext = this;
        }

        private string selectedLanguage;
        public string SelectedLanguage
        {
            get { return selectedLanguage; }
            set { selectedLanguage = value; RaisePropertyChanged(nameof(SelectedLanguage)); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));


        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
