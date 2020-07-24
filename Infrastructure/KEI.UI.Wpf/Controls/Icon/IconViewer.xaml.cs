using KEI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KEI.UI.Wpf.Controls.Icon
{
    /// <summary>
    /// Interaction logic for IconViewer.xaml
    /// </summary>
    public partial class IconViewer : Window , INotifyPropertyChanged
    {

        private string filter;

        public string Filter
        {
            get { return filter; }
            set
            {
                filter = value;
                UpdateFilter();
            }
        }
        public Infrastructure.Screen.Icon SelectedIcon { get; set; }

        private int currentPage;
        public int CurrentPage
        {
            get => currentPage;
            set
            {
                currentPage = value;

                Page.Clear();
                Page.AddRange(FilteredIcons.Skip((CurrentPage - 1) * ItemsPerPage).Take(ItemsPerPage));

                if (CurrentPage == MaxPages)
                    nextBtn.IsEnabled = false;
                else if (CurrentPage == 1)
                    prevBtn.IsEnabled = false;
                else
                    nextBtn.IsEnabled = prevBtn.IsEnabled = true;

                RaisePropertyChanged(nameof(CurrentPage));
            }
        } 
        public int ItemsPerPage { get; set; } = 42;
        public int MaxPages { get; set; }

        private void UpdateFilter()
        {

            ThreadingHelper.AsyncDispatcherInvoke(() =>
            {
                FilteredIcons.Clear();
                if (string.IsNullOrEmpty(filter))
                {
                    FilteredIcons = new ObservableCollection<Infrastructure.Screen.Icon>(AvailableIcons);
                    RaisePropertyChanged(nameof(FilteredIcons));
                }
                else
                    FilteredIcons.AddRange(AvailableIcons.Where(x => x.ToString().ToLower().Contains(filter.ToLower())));

                CurrentPage = 1;

                if (FilteredIcons.Count % ItemsPerPage > 0)
                {
                    MaxPages = (FilteredIcons.Count / ItemsPerPage) + 1;
                }
                else
                {
                    MaxPages = (FilteredIcons.Count / ItemsPerPage);
                }

            });
        }
        public IconViewer()
        {
            InitializeComponent();
            DataContext = this;


            ThreadingHelper.AsyncDispatcherInvoke(() => 
            {
                foreach (var item in Enum.GetValues(typeof(Infrastructure.Screen.Icon)))
                {
                    AvailableIcons.Add((KEI.Infrastructure.Screen.Icon)item);
                    FilteredIcons.Add((KEI.Infrastructure.Screen.Icon)item);
                }
                CurrentPage = 1;
                if (FilteredIcons.Count % ItemsPerPage > 0)
                {
                    MaxPages = FilteredIcons.Count / ItemsPerPage + 1;
                }
                else
                {
                    MaxPages = FilteredIcons.Count / ItemsPerPage;
                }
            });


        }

        public ObservableCollection<KEI.Infrastructure.Screen.Icon> FilteredIcons { get; set; } = new ObservableCollection<Infrastructure.Screen.Icon>();
        public ObservableCollection<KEI.Infrastructure.Screen.Icon> Page { get; set; } = new ObservableCollection<Infrastructure.Screen.Icon>();
        public List<KEI.Infrastructure.Screen.Icon> AvailableIcons { get; set; } = new List<Infrastructure.Screen.Icon>();

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SelectedIcon = (Infrastructure.Screen.Icon)(sender as Border).DataContext;
            DialogResult = true;
            Close();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentPage < MaxPages)
            {
                CurrentPage++;
            }

        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentPage > 1)
            {
                CurrentPage--;
            }
        }
    }
}
