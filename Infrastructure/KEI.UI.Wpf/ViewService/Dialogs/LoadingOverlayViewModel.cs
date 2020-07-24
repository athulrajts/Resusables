using CommonServiceLocator;
using KEI.Infrastructure.Events;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows;

namespace KEI.UI.Wpf.ViewService.Dialogs
{
    public class LoadingOverlayViewModel : BindableBase, IDialogAware
    {
        public string Title => "Application Busy";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            LoadingText = parameters.GetValue<string[]>("text");
        }

        private string[] loadingText;
        public string[] LoadingText
        {
            get { return loadingText; }
            set { SetProperty(ref loadingText, value); }
        }

        public LoadingOverlayViewModel()
        { 
            ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<SetAvailableEvent>().Subscribe(() => RequestClose?.Invoke(new DialogResult(ButtonResult.OK)), ThreadOption.UIThread);
            ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<UpdateBusyText>().Subscribe(msg => LoadingText = msg, ThreadOption.UIThread);
            WindowState = Application.Current.MainWindow.WindowState;
            Width = Application.Current.MainWindow.ActualWidth;
            Height = Application.Current.MainWindow.ActualHeight;
            if (WindowState == WindowState.Normal)
            {
                Top = Application.Current.MainWindow.Top;
                Left = Application.Current.MainWindow.Left;
            }
        }

        private double top;
        public double Top
        {
            get { return top; }
            set { SetProperty(ref top, value); }
        }

        private double left;
        public double Left
        {
            get { return left; }
            set { SetProperty(ref left, value); }
        }

        private double height;
        public double Height
        {
            get { return height; }
            set { SetProperty(ref height, value); }
        }

        private double width;
        public double Width
        {
            get { return width; }
            set { SetProperty(ref width, value); }
        }

        private WindowState windowState;
        public WindowState WindowState
        {
            get { return windowState; }
            set { SetProperty(ref windowState, value); }
        }
    }
}
