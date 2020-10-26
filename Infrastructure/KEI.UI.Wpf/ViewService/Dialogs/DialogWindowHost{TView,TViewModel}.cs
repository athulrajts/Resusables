using System;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using Prism.Ioc;
using Prism.Services.Dialogs;

namespace KEI.UI.Wpf
{
    public class DialogWindowHost<TView,TViewModel> : DialogWindowHostBase
        where TView : UserControl
        where TViewModel : INotifyPropertyChanged
    {
        public DialogWindowHost(IDialogParameters parameters = null) : base(parameters)
        {
            InitializeComponents();

            DataContext = ContainerLocator.Container.Resolve<TViewModel>();

            (Content as UserControl).DataContext = DataContext;
        }


        public DialogWindowHost(TViewModel viewModel, IDialogParameters parameters = null) : base(parameters)
        {
            InitializeComponents();
            
            DataContext = viewModel;

            (Content as UserControl).DataContext = DataContext;
        }

        private void InitializeComponents()
        {
            Content = ContainerLocator.Container.Resolve<TView>();

            WindowStartupLocation = Dialog.GetWindowStartupLocation(Content as DependencyObject);
            if (Dialog.GetWindowStyle(Content as DependencyObject) is Style s)
            {
                Style = s;
            }
        }

    }
}
