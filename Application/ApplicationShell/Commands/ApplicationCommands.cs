using Application.Core;
using Application.Core.Interfaces;
using KEI.Infrastructure;
using KEI.Infrastructure.Database;
using Prism.Commands;

namespace ApplicationShell.Commands
{
    public class ApplicationCommands
    {
        private readonly IEquipment _equipment;
        private readonly IDatabaseManager _databaseManager;
        private readonly IViewService _viewService;
        public ApplicationCommands(IViewService viewService, IDatabaseManager databaseManager, IEquipment equipment)
        {
            _viewService = viewService;
            _databaseManager = databaseManager;
            _equipment = equipment;
        }

        #region Exit Application

        private DelegateCommand exitApplictionCommand;
        public DelegateCommand ExitApplicationCommand =>
            exitApplictionCommand ?? (exitApplictionCommand = new DelegateCommand(ExecuteExitApplicationCommand));
        private void ExecuteExitApplicationCommand()
        {
            // TODO : Can Exit Check;

            if (_viewService.Prompt("Are you sure you want to exit application ?", PromptOptions.YesNo) == PromptResult.Yes)
                System.Windows.Application.Current.Shutdown();
        }

        #endregion

        #region Start Test

        private DelegateCommand startTest;
        public DelegateCommand StartTestCommand =>
            startTest ??= new DelegateCommand(ExecuteStartTestCommand);

        void ExecuteStartTestCommand()
        {
            _databaseManager[$"{ApplicationMode.Engineering} DB"].StartSession(_equipment.CurrentDatabaseSetup);

            _equipment.ExecuteTest();
        }

        #endregion
    }
}
