using CommonServiceLocator;
using Prism.Services.Dialogs;
using System;
using System.Windows;
using System.Windows.Controls;

namespace KEI.UI.Wpf
{
    public class DialogWindowHost<TView> : DialogWindowHostBase
        where TView : UserControl
    {

        public DialogWindowHost(IDialogParameters parameters = null) : base(parameters)
        {
            Content = ServiceLocator.IsLocationProviderSet
                ? ServiceLocator.Current.GetInstance<TView>()
                : Activator.CreateInstance<TView>();

            WindowStartupLocation = Dialog.GetWindowStartupLocation(Content as DependencyObject);
            if (Dialog.GetWindowStyle(Content as DependencyObject) is Style s)
            {
                Style = s;
            }

            DataContext = (Content as UserControl).DataContext;
        }
    }
}
