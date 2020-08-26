using KEI.Infrastructure.Configuration;
using KEI.UI.Wpf.ViewService.ViewModels;
using Prism.Services.Dialogs;
using System.Collections.Generic;

namespace KEI.UI.Wpf.ViewService
{
    public class ConfigsChangedDialogViewModel : BaseDialogViewModel
    {

        public override string Title { get; set; } = "Values Changed";

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            History.Clear();

            History = parameters.GetValue<Dictionary<string, Dictionary<string, ConfigHistoryItem>>>("changes");
        }

        private Dictionary<string, Dictionary<string, ConfigHistoryItem>> history = new Dictionary<string, Dictionary<string, ConfigHistoryItem>>();
        public Dictionary<string, Dictionary<string, ConfigHistoryItem>> History
        {
            get => history;
            set => SetProperty(ref history, value);
        } 
    }
}
