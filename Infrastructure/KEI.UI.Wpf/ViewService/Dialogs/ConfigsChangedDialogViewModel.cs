using KEI.Infrastructure.Configuration;
using KEI.Infrastructure.Localizer;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;

namespace KEI.UI.Wpf.ViewService
{
    public class ConfigsChangedDialogViewModel : BindableBase, IDialogAware
    {
        private DelegateCommand<string> _closeDialogCommand;
        public DelegateCommand<string> CloseDialogCommand =>
            _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand<string>(CloseDialog));

        public string Title => "Values Changed";

        public event Action<IDialogResult> RequestClose;

        public ConfigsChangedDialogViewModel()
        {
        }

        protected virtual void CloseDialog(string parameter)
        {
            ButtonResult result = ButtonResult.OK;

            if (parameter.ToLower() == "yes")
                result = ButtonResult.Yes;
            else if (parameter.ToLower() == "no")
                result = ButtonResult.No;

            RaiseRequestClose(new DialogResult(result));
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {

        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            History.Clear();

            History = (Dictionary<string, Dictionary<string, ConfigHistoryItem>>)parameters.GetValue<object>("changes");
        }

        private Dictionary<string, Dictionary<string, ConfigHistoryItem>> history = new Dictionary<string, Dictionary<string, ConfigHistoryItem>>();
        public Dictionary<string, Dictionary<string, ConfigHistoryItem>> History
        {
            get => history;
            set => SetProperty(ref history, value);
        } 
    }
}
