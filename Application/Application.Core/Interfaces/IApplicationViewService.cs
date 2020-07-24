using Application.Core.Models;
using KEI.Infrastructure;
using KEI.Infrastructure.Configuration;
using KEI.Infrastructure.Service;
using Prism.Services.Dialogs;
using System.Collections.Generic;

namespace Application.Core
{
    [Service("View Service", null)]
    public interface IApplicationViewService : IViewService
    {
        PromptResult ShowUpdatedConfigs(Dictionary<string,Dictionary<string,ConfigHistoryItem>> history);
        void ShowAdvancedSetup();
        ButtonResult StartTestDialog();
    }
}
