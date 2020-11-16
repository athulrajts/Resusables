using KEI.Infrastructure;
using KEI.Infrastructure.Configuration;
using KEI.Infrastructure.Service;
using System.Collections.Generic;

namespace Application.Core
{
    [Service("View Service")]
    public interface IApplicationViewService : IViewService
    {
        PromptResult ShowUpdatedConfigs(Dictionary<string,Dictionary<string,ConfigHistoryItem>> history);
        void ShowAdvancedSetup();
        PromptResult StartTestDialog();
    }
}
