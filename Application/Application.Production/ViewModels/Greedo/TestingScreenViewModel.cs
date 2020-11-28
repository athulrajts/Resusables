using System.Data;
using System.Collections.Generic;
using Prism.Events;
using Prism.Commands;
using KEI.Infrastructure;
using KEI.Infrastructure.Prism;
using KEI.Infrastructure.Screen;
using KEI.Infrastructure.Events;
using KEI.Infrastructure.Database;
using KEI.UI.Wpf.Hotkey;
using Application.Core;
using Application.Core.Interfaces;
using Application.Production.Views;
using Application.Production.Screen;

namespace Application.Production.ViewModels
{
    [Screen(DisplayName = ScreenDisplayNames.TestingScreen,
            Icon = Icon.Test16x,
            ScreenName = nameof(TestingScreen))]
    public class TestingScreenViewModel : BaseScreenViewModel
    {

        private IPropertyContainer _currentRecipe;
        private IDatabase _mainDatabase;
        private IEquipment _equipment;

        #region Constructor
        public TestingScreenViewModel(IEventAggregator eventAggregator, IDatabaseManager dbm, IEquipment equipment,
            IHotkeyService hotkeyService) : base(hotkeyService)
        {
            _equipment = equipment;
            _mainDatabase = dbm[$"{ApplicationMode.Production} DB"];

            Data = _mainDatabase.GetData();

            Commands.Add(new ScreenCommand
            {
                DisplayName = "Test",
                Command = TestCommand
            });

            eventAggregator.GetEvent<RecipeLoadedEvent>().Subscribe((recipe) => _currentRecipe = recipe);
            eventAggregator.GetEvent<TestExecuted>().Subscribe(result =>
            {
                ImagePath = result.Item2;
                Results = result.Item3;
                IsPass = Results.TrueForAll(x => x.IsPass == true);
                ResultText = IsPass ? "PASS" : "FAIL";
            }, ThreadOption.BackgroundThread, false,
            (result) => result.Item1 == ApplicationMode.Production);
        }

        #endregion

        #region Properties

        private string imagePath;
        public string ImagePath
        {
            get => imagePath;
            set => SetProperty(ref imagePath, value);
        }

        private List<TestResult> results = new List<TestResult>();
        public List<TestResult> Results
        {
            get => results;
            set => SetProperty(ref results, value);
        }

        private bool isPass;
        public bool IsPass
        {
            get => isPass;
            set => SetProperty(ref isPass, value);
        }

        private string resultText;
        public string ResultText
        {
            get => resultText;
            set => SetProperty(ref resultText, value);
        }

        private DataTable data;
        public DataTable Data
        {
            get { return data; }
            set { SetProperty(ref data, value); }
        }

        #endregion

        #region Test Command

        private DelegateCommand testCommand;
        public DelegateCommand TestCommand =>
            testCommand ?? (testCommand = new DelegateCommand(ExecuteTestCommand));
        private void ExecuteTestCommand()
        {
            _mainDatabase.StartSession(_equipment.CurrentDatabaseSetup.Morph() as DatabaseSetup);

            _equipment.ExecuteTest();

            Data = _mainDatabase.GetData();
        }

        #endregion

    }
}
