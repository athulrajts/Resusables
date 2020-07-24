using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Windows;
using System.Windows.Controls;

namespace KEI.UI.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for BrowseTextBox.xaml
    /// </summary>
    public partial class BrowseTextBox : UserControl
    {
        public BrowseTextBox()
        {
            InitializeComponent();
 
        }

        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(string), typeof(BrowseTextBox), new PropertyMetadata(string.Empty, OnPathChanged));

        private static void OnPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
             var bt = d as BrowseTextBox;
            if (e.NewValue != e.OldValue && e.NewValue is string path)
            {
                bt.Path = GetRelativePath(path, AppDomain.CurrentDomain.BaseDirectory);
            }
        }

        public BrowseType Type
        {
            get { return (BrowseType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(BrowseType), typeof(BrowseTextBox), new PropertyMetadata(BrowseType.Folder));



        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(string), typeof(BrowseTextBox), new PropertyMetadata(""));



        public string FilterDescription
        {
            get { return (string)GetValue(FilterDescriptionProperty); }
            set { SetValue(FilterDescriptionProperty, value); }
        }

        public static readonly DependencyProperty FilterDescriptionProperty =
            DependencyProperty.Register("FilterDescription", typeof(string), typeof(BrowseTextBox), new PropertyMetadata(""));



        public static string GetRelativePath(string path, string root)
        {
            try
            {
                if (!System.IO.Path.IsPathRooted(path))
                    return path;

                var pathURI = new Uri(path);
                var rootURI = new Uri(root);
                return rootURI.MakeRelativeUri(pathURI).ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;
            dlg.IsFolderPicker = Type == BrowseType.Folder ? true : false;
            dlg.DefaultDirectory = AppDomain.CurrentDomain.BaseDirectory;
            dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (!string.IsNullOrEmpty(FilterDescription) && !string.IsNullOrEmpty(Filter) && Type == BrowseType.File)
            {
                dlg.Filters.Add(new CommonFileDialogFilter(FilterDescription, Filter));
            }

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folder = dlg.FileName;
                Path = dlg.FileName;
            }
        }
    }

    public enum BrowseType
    {
        File,
        Folder
    }
}
