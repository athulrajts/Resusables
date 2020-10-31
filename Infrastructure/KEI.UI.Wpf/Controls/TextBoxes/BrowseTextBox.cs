using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace KEI.UI.Wpf.Controls
{
    [TemplatePart(Name = PART_BrowseButton, Type = typeof(ButtonBase))]
    public class BrowseTextBox : TextBox
    {
        private const string PART_BrowseButton = "PART_BrowseButton";

        private ButtonBase _button;
        private ButtonBase Button
        {
            get { return _button; }
            set 
            {
                if (_button != null)
                {
                    _button.Click -= BrowseAction;
                }

                _button = value;

                if(_button != null)
                {
                    _button.Click += BrowseAction;
                }
            }
        }

        public override void OnApplyTemplate()
        {
            Button = GetTemplateChild(PART_BrowseButton) as ButtonBase;

            base.OnApplyTemplate();
        }

        static BrowseTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BrowseTextBox), new FrameworkPropertyMetadata(typeof(BrowseTextBox)));
            TextProperty.OverrideMetadata(typeof(BrowseTextBox), new FrameworkPropertyMetadata("", OnTextChanged));
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bt = d as BrowseTextBox;
            if (e.NewValue != e.OldValue && e.NewValue is string path)
            {
                bt.Text = GetRelativePath(path, AppDomain.CurrentDomain.BaseDirectory);
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



        public double BrowseButtonWidth
        {
            get { return (double)GetValue(BrowseButtonWidthProperty); }
            set { SetValue(BrowseButtonWidthProperty, value); }
        }

        public static readonly DependencyProperty BrowseButtonWidthProperty =
            DependencyProperty.Register("BrowseButtonWidth", typeof(double), typeof(BrowseTextBox), new PropertyMetadata(75.0));



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


        private void BrowseAction(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog
            {
                AddToMostRecentlyUsedList = false,
                AllowNonFileSystemItems = false,
                EnsureFileExists = true,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true,
                Multiselect = false,
                ShowPlacesList = true,
                IsFolderPicker = Type == BrowseType.Folder ? true : false,
                DefaultDirectory = AppDomain.CurrentDomain.BaseDirectory,
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };

            if (!string.IsNullOrEmpty(FilterDescription) && !string.IsNullOrEmpty(Filter) && Type == BrowseType.File)
            {
                dlg.Filters.Add(new CommonFileDialogFilter(FilterDescription, Filter));
            }

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Text = dlg.FileName;
            }
        }
    }

    public enum BrowseType
    {
        File,
        Folder
    }
}
