using System.IO;
using System.Data;
using System.Linq;
using System.Diagnostics;
using System.Windows.Input;
using Prism.Commands;
using Application.Core.Interfaces;
using Application.Production.Views;
using Application.UI.AdvancedSetup.ViewModels;
using KEI.Infrastructure;
using KEI.Infrastructure.Configuration;
using KEI.Infrastructure.Database;
using KEI.Infrastructure.Prism;
using KEI.Infrastructure.Screen;
using KEI.Infrastructure.Utils;
using KEI.UI.Wpf;
using KEI.UI.Wpf.Hotkey;

namespace Application.Production.ViewModels
{
    [Screen(DisplayName = "Demos 2",
         Icon = Icon.AzureSQLDatabase16x,
         ScreenName = nameof(DemoScreen2))]
    public class DemoScreen2ViewModel : BaseScreenViewModel
    {
        private DatabaseSetup setup;

        private readonly IDatabaseManager _databaseManager;
        private readonly IViewService _viewService;
        private readonly IFileDatabase _database;

        public DemoScreen2ViewModel(IHotkeyService hotkeyService, 
            IDatabaseManager databaseManager, IViewService viewService) : base(hotkeyService)
        {
            _databaseManager = databaseManager;
            _viewService = viewService;
            _database = new Database(new CSVDatabaseWritter());

            InitDatabase();

            InitCommands();
        }

        private DataTable data;
        public DataTable Data
        {
            get { return data; }
            set { SetProperty(ref data, value); }
        }

        private DatabaseItem databaseObject = new DatabaseItem();
        public DatabaseItem DatabaseObject
        {
            get { return databaseObject; }
            set { SetProperty(ref databaseObject, value); }
        }

        public ICommand EditDatabaseCommand { get; set; }
        public ICommand LogRandomCommand { get; set; }
        public ICommand OpenDBFileCommand { get; set; }
        public ICommand LogNowCommand { get; set; }

        private void InitDatabase()
        {
            var dbSetupPath = PathUtils.GetPath("Configs/setup.xcfg");
            var dbPath = PathUtils.GetPath("Database/Test.csv");
            if (File.Exists(dbSetupPath))
            {
                setup = DataContainerBuilder.FromFile(dbSetupPath).Morph<DatabaseSetup>();
            }
            else
            {
                setup = new DatabaseSetup
                {
                    CreationMode = DatabaseCreationMode.Daily,
                    Name = dbPath,
                    Columns = DatabaseColumnGenerator.ColumnsFor<DatabaseItem>()
                };

                var config = DataContainerBuilder.CreateObject("DB", setup);
                config.Store(dbSetupPath);
            }

            _database.StartSession(setup);

            Data = _database.GetData();
        }

        private void InitCommands()
        {
            EditDatabaseCommand = new DelegateCommand(() =>
            {
                var setupConfig = DataContainerBuilder.CreateObject("DB", setup);
                var vm = new DatabaseSetupViewModel(_viewService, setupConfig, typeof(DatabaseItem));

                // for demo, shoud not reference UI in viewmodel
                var dialog = new DialogWindowHost<UI.AdvancedSetup.Views.DatabaseSetupView, DatabaseSetupViewModel>(vm);
                dialog.ShowDialog();

                _database.StartSession(setupConfig.Morph<DatabaseSetup>());

                Data = _database.GetData();
            });

            LogRandomCommand = new DelegateCommand(() => _database.AddRecord(DatabaseItem.GetRandom()));

            OpenDBFileCommand = new DelegateCommand(() => 
            {
                try
                {
                    Process.Start("notepad", _database.GetFilePath());
                }
                catch (System.Exception) { }
            });

            LogNowCommand = new DelegateCommand(() => _database.AddRecord(DatabaseObject));
        }
    }
}
