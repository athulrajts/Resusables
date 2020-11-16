using Prism.Ioc;
using Prism.Services.Dialogs;
using System;
using System.Windows;
using System.Windows.Media;

namespace KEI.UI.Wpf.ViewService.Dialogs
{
    public class OverlayWindow<TContent, TViewModel> : Window
        where TContent : FrameworkElement
    {
        private readonly Window parentWindow;

        private readonly IDialogParameters parameters;

        public OverlayWindow(IDialogParameters param)
        {
            var vm = ContainerLocator.Container.Resolve<TViewModel>();

            DataContext = vm;

            Content = Activator.CreateInstance<TContent>();

            SizeToContent = SizeToContent.Manual;
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            Background = Brushes.Transparent;

            parameters = param;

            parentWindow = Application.Current.MainWindow;
            Owner = parentWindow;
            parentWindow.Closed += Parent_Closed;
            parentWindow.LocationChanged += Parent_LocationChanged;
            parentWindow.SizeChanged += ParentWindow_SizeChanged;
            Loaded += OverlayWindow_Loaded;

            Height = parentWindow.ActualHeight;
            Width = parentWindow.ActualWidth;
            Top = parentWindow.Top;
            Left = parentWindow.Left;
            WindowState = parentWindow.WindowState;

        }

        private void OverlayWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is IDialogAware vm &&
               parameters != null)
            {
                vm.OnDialogOpened(parameters);
            }
        }

        private void ParentWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Height = e.NewSize.Height;
            Width = e.NewSize.Width;
        }

        private void Parent_LocationChanged(object sender, EventArgs e)
        {
            Top = parentWindow.Top;
            Left = parentWindow.Left;
            WindowState = parentWindow.WindowState;
        }

        private void Parent_Closed(object sender, EventArgs e)
        {
            Close();
        }
    }
}
