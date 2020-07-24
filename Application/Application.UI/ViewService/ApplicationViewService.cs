using Application.Core;
using Application.Core.Models;
using KEI.Infrastructure;
using KEI.Infrastructure.Configuration;
using KEI.Infrastructure.Events;
using KEI.UI.Wpf.ViewService;
using Prism.Events;
using Prism.Services.Dialogs;
using System.Collections.Generic;

namespace Application.UI
{
    public class ApplicationViewService : BaseViewService, IApplicationViewService
    {
        private IPropertyContainer _recipe;
        public ApplicationViewService(IDialogService dialogService, ILogManager logManager, IEventAggregator ea) : base(dialogService, logManager, ea)
        {
            ea.GetEvent<RecipeLoadedEvent>().Subscribe((recipe) => _recipe = recipe);
        }


        public PromptResult ShowUpdatedConfigs(Dictionary<string, Dictionary<string, ConfigHistoryItem>> history)
        {
            DialogParameters @params = new DialogParameters();
            @params.Add("changes", history);

            var result = PromptResult.Yes;

            _dialogService.ShowDialog("ConfigsChangedDialog", @params, r => { result = (PromptResult)(int)r.Result; });

            return result;
        }

        public void ShowAdvancedSetup()
        {
            _dialogService.ShowDialog(typeof(AdvancedSetup.AdvancedSetupDialog).Name, null, r => { });
        }

        public ButtonResult StartTestDialog()
        { 
            ButtonResult result = ButtonResult.OK;

            var param = new DialogParameters();
            var spec = new TestSpecification();
            param.Add("Spec", spec);

            _dialogService.ShowDialog(typeof(ViewService.StartTestDialog).Name, param, r => { result = r.Result; });

            return result;
        }
    }
}
