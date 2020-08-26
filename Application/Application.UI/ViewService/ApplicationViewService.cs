using Application.Core;
using Application.Core.Models;
using Application.UI.AdvancedSetup;
using KEI.Infrastructure;
using KEI.Infrastructure.Configuration;
using KEI.Infrastructure.Events;
using KEI.UI.Wpf;
using KEI.UI.Wpf.ViewService;
using Prism.Events;
using Prism.Services.Dialogs;
using System.Collections.Generic;

namespace Application.UI
{
    public class ApplicationViewService : BaseViewService, IApplicationViewService
    {
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;
        private IPropertyContainer _recipe;
        
        public ApplicationViewService(IDialogService dialogService, ILogManager logManager, IEventAggregator ea)
        {
            _dialogService = dialogService;
            _eventAggregator = ea;
            
            _eventAggregator.GetEvent<RecipeLoadedEvent>().Subscribe((recipe) => _recipe = recipe);
        }


        public PromptResult ShowUpdatedConfigs(Dictionary<string, Dictionary<string, ConfigHistoryItem>> history)
        {
            DialogParameters parameters = new DialogParameters
            {
                { "changes", history }
            };


            var dialog = new DialogWindowHost<ConfigsChangedDialog, ConfigsChangedDialogViewModel>(parameters);

            dialog.ShowDialog();
             
            return dialog.Result;
        }

        public void ShowAdvancedSetup()
        {
            var dialog = new DialogWindowHost<AdvancedSetupDialog, AdvancedSetupDialogViewModel>();

            dialog.ShowDialog();

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
