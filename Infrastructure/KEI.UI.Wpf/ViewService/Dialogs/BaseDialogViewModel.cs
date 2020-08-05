using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KEI.UI.Wpf.ViewService
{
    public abstract class BaseDialogViewModel : BindableBase, IDialogViewModel
    {
        private DelegateCommand<string> _closeDialogCommand;
        public DelegateCommand<string> CloseDialogCommand =>
            _closeDialogCommand ??= _closeDialogCommand = new DelegateCommand<string>(CloseDialog);

        public bool IsOpen { get; set; } = true;

        protected virtual async void CloseDialog(string parameter)
        {
            ButtonResult result = (parameter?.ToLower()) switch
            {
                "ok" => ButtonResult.OK,
                "cancel" => ButtonResult.Cancel,
                "yes" => ButtonResult.Yes,
                "no" => ButtonResult.No,
                "ignore" => ButtonResult.Ignore,
                "retry" => ButtonResult.Retry,
                "abort" => ButtonResult.Abort,
                _ => ButtonResult.None,
            };
            
            CloseDialogAnimation?.Invoke();

            await Task.Delay(200);

            IsOpen = false;
            
            RequestClose?.Invoke(new DialogResult(result));
        }

        public Action CloseDialogAnimation { get; set; }

        public abstract string Title { get; set; }

        public event Action<IDialogResult> RequestClose;

        public virtual bool CanCloseDialog() => true;

        public virtual void OnDialogClosed() { }

        public abstract void OnDialogOpened(IDialogParameters parameters);
    }
}
