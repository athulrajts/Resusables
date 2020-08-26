using KEI.Infrastructure;
using Prism.Services.Dialogs;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;

namespace KEI.UI.Wpf
{
    public abstract class DialogWindowHostBase : Window
    {
        public PromptResult Result { get; set; }
        public IDialogParameters Parameters { get; set; }

        public DialogWindowHostBase(IDialogParameters parameters)
        {
            Parameters = parameters;

            DataContextChanged += DialogWindowHost_DataContextChanged;
            Loaded += DialogWindowHost_Loaded;
            Closing += DialogWindowHost_Closing;
        }

        public void ShowDialog(bool isModal)
        {
            if (isModal)
            {
                ShowDialog();
            }
            else
            {
                Show();
            }
        }

        protected virtual void DialogWindowHost_Closing(object sender, CancelEventArgs e)
        {
            if (DataContext is IDialogAware vm)
            {
                vm.OnDialogClosed();
            }
        }

        protected virtual void DialogWindowHost_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is IDialogAware vm &&
               Parameters != null)
            {
                vm.OnDialogOpened(Parameters);
            }
        }

        protected virtual void DialogWindowHost_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is IDialogAware newVM)
            {
                newVM.RequestClose += CloseDialog;
            }
            if (e.OldValue is IDialogAware oldVM)
            {
                oldVM.RequestClose -= CloseDialog;
            }
        }

        protected virtual void CloseDialog(IDialogResult result)
        {
            Result = (PromptResult)(int)result.Result;

            if (ComponentDispatcher.IsThreadModal)
            {
                DialogResult = true;
            }
        }
    }
}
