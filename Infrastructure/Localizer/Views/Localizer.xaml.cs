using KEI.Infrastructure;
using Localizer.Core;
using Localizer.ViewModels;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Localizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LocalizerWindow : Window, INotifyPropertyChanged
    {
        public LocalizerWindow(LocalizerViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

        }

        public ObservableCollection<Translation> Resource { get; set; } = new ObservableCollection<Translation>();

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var translater = new Translator();

            foreach (var item in Resource)
            {
                if (string.IsNullOrEmpty(item.TranslatedText))
                {
                    item.TranslatedText = await Task.Run(() => translater.Translate(item.EnglishText, "English", "French"));
                    await Task.Delay(1000);
                }
            }
        }
    }
}
