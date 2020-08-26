using System;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using CommonServiceLocator;
using Prism.Services.Dialogs;

namespace KEI.UI.Wpf
{
    public class DialogWindowHost<TView,TViewModel> : DialogWindowHostBase
        where TView : UserControl
        where TViewModel : INotifyPropertyChanged
    {
        public DialogWindowHost(IDialogParameters parameters = null) : base(parameters)
        {
            InitializeComponents(parameters);

            DataContext = ServiceLocator.IsLocationProviderSet
                ? ServiceLocator.Current.GetInstance<TViewModel>()
                : Activator.CreateInstance<TViewModel>();

            (Content as UserControl).DataContext = DataContext;
        }


        public DialogWindowHost(TViewModel viewModel, IDialogParameters parameters) : base(parameters)
        {
            InitializeComponents(parameters);
            
            DataContext = viewModel;

            (Content as UserControl).DataContext = DataContext;
        }

        private void InitializeComponents(IDialogParameters parameters)
        {
            Content = ServiceLocator.IsLocationProviderSet
                ? ServiceLocator.Current.GetInstance<TView>()
                : Activator.CreateInstance<TView>();

            WindowStartupLocation = Dialog.GetWindowStartupLocation(Content as DependencyObject);
            if (Dialog.GetWindowStyle(Content as DependencyObject) is Style s)
            {
                Style = s;
            }
        }

    }
}
