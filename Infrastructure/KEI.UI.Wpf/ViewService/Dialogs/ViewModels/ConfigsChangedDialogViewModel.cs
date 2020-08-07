using KEI.Infrastructure.Configuration;
using KEI.Infrastructure.Localizer;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;

namespace KEI.UI.Wpf.ViewService
{
    public class ConfigsChangedDialogViewModel : BaseDialogViewModel
    {

        public override string Title { get; set; } = "Values Changed";

        public override void OnDialogOpened(IDialogParameters parameters)
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
