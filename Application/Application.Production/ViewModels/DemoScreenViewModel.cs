using System.IO;
using System.Linq;
using System.Data;
using System.Windows.Input;
using System.Threading.Tasks;
using Prism.Commands;
using KEI.Infrastructure;
using KEI.Infrastructure.Prism;
using KEI.Infrastructure.Screen;
using KEI.Infrastructure.Database;
using KEI.Infrastructure.Localizer;
using KEI.Infrastructure.Validation;
using KEI.Infrastructure.Configuration;
using KEI.UI.Wpf;
using KEI.UI.Wpf.Hotkey;
using Application.Core;
using Application.Core.Constants;
using Application.UI.AdvancedSetup.ViewModels;
using Application.Production.Views;
using KEI.Infrastructure.Utils;
using System;

namespace Application.Production.ViewModels
{
    [Screen(DisplayName = "Demos",
             Icon = Icon.ShowAllCode16x,
             ScreenName = nameof(DemoScreen))]
    public class DemoScreenViewModel : BaseScreenViewModel<DemoScreen>
    {
        public override string DisplayName { get; set; } = "Demos";
        public override Icon Icon { get; set; } = Icon.ShowAllCode16x;

        private readonly IViewService _viewService;
        private readonly ISystemStatusManager _statusManager;
        private readonly IStringLocalizer _localizer;
        private readonly IDatabase _database;
        private DatabaseSetup setup;
        public DemoScreenViewModel(IHotkeyService hotkeyService, IViewService viewService,
            ISystemStatusManager statusManager, IConfigManager configManager,
            ILocalizationProvider localizationProvider) : base(hotkeyService)
        {
            _viewService = viewService;
            _statusManager = statusManager;
            _localizer = localizationProvider.Provide();

            var generalPerferences = configManager.GetConfig(ConfigKeys.GeneralPreferences);

            SamplePropertyContainer = PropertyContainerBuilder.Create("Config").WithProperty("PropertyOne", true).Build();
            SamplePropertyContainerTwo = PropertyContainerBuilder.Create("Sample Confing")
                .WithProperty("IntegerProperty", 22, "Integer Property")
                .WithProperty("FloalProperty", 3.14, "Float Property")
                .WithProperty("DoubleProperty", 3.14142, "Double Property")
                .WithProperty("BoolProperty", false, "Boolean Property")
                .WithProperty("StringProperty", "Hello World", "String Property")
                .WithFile("FileProperty", @".\Configs\DefaultRecipe.rcp", "File Path Property")
                .WithFolder("FolderProperty", @".\Configs", "Folder Path Property")
                .WithEnum("EnumProperty", ApplicationMode.Production, "Enum Property")
                .WithObject("Object Property", this)
                .Build();

            IPropertyContainer samplePropertyContainerThree = new PropertyDictionary
            {
                {"IntegerProperty", 22 },
                {"FloalProperty", 3.14},
                {"DoubleProperty", 3.14142 },
                {"BoolProperty", false },
                {"StringProperty", "Hello World" },
                {"EnumProperty", ApplicationMode.Production }
            };

            ValidationRule = ValidationBuilder.Create(true)
                .Length(5)
                .Length(2, 10)
                .MaxLength(7)
                .MinLength(3)
                .NotNullOrEmpty()
                .Directory()
                .File()
                .Range(10, 100)
                .OutsideRange(10, 100)
                .Positive()
                .Negative()
                .Type(typeof(int))
                .Type(typeof(double))
                .Type(typeof(float))
                .Type(typeof(bool));

            SamplePropertyContainer.SetBinding("PropertyOne", () => OneTimeBinding, BindingMode.OneTime);
            SamplePropertyContainer.SetBinding("PropertyOne", () => OneWayBinding, BindingMode.OneWay);
            SamplePropertyContainer.SetBinding("PropertyOne", () => TwoWayBinding, BindingMode.TwoWay);
            SamplePropertyContainer.SetBinding("PropertyOne", () => OneWayToSourceBinding, BindingMode.OneWayToSource);

            generalPerferences.SetBinding("Theme", () => Theme, BindingMode.OneWayToSource);

            InitCommands();

            var dbSetupPath = PathUtils.GetPath("Configs/setup.xcfg");
            var dbPath = PathUtils.GetPath("Database/Test.csv");
            if (File.Exists(dbSetupPath))
            {
                setup = PropertyContainerBuilder.FromFile(dbSetupPath).Morph<DatabaseSetup>();
            }
            else
            {
                setup = new DatabaseSetup
                {
                    CreationMode = DatabaseCreationMode.Daily,
                    Name = dbPath,
                    Schema = DatabaseSchema.SchemaFor<DatabaseItem>().ToList()
                };

                setup.ToPropertyContainer("DB").Store(dbSetupPath);
            }

            _database = new Database(new CSVDatabaseWritter());
            _database.StartSession(setup);

            _database.AddRecord(DatabaseItem.GetRandom());

            Data = _database.GetData();

            ValidationRule.Validate(ValidatingText);

            RaisePropertyChanged(string.Empty);
        }

        private void UpdateValidation()
        {
            ValidationRule.Validate(ValidatingText);
            RaisePropertyChanged(nameof(ValidationRule));
        }
        private void InitCommands()
        {
            ShowInfoCommand = new DelegateCommand(() => _viewService.Inform(DialogText, true));
            ShowWarningCommand = new DelegateCommand(() => _viewService.Warn(DialogText));
            ShowErrorCommand = new DelegateCommand(() => _viewService.Error(DialogText));
            SetBusyCommand = new DelegateCommand(async () =>
            {
                _viewService.SetBusy("Loading", "5");
                for (int i = 5; i > 0; i--)
                {
                    await Task.Delay(1000);
                    _viewService.UpdateBusyText("Loading", $"{i - 1}");
                }
                _viewService.SetAvailable();
            });
            ShowPromptCommand = new DelegateCommand<object>(option =>
            {
                if (option is PromptOptions po)
                {
                    //_viewService.Prompt(DialogText, po); 
                    _viewService.PromptWithDefault(DialogText, po, PromptResult.Retry, TimeSpan.FromSeconds(5));
                }
            });

            EditDatabaseCommand = new DelegateCommand(() =>
            {
                //new System.Windows.Window
                //{
                //    Content = new UI.AdvancedSetup.Views.DatabaseSetupView()
                //    {
                //        DataContext = new DatabaseSetupViewModel(setup)
                //    },
                //    Width = 850,
                //    Height = 500
                //}.ShowDialog();
            });

            LogNowCommand = new DelegateCommand(() => _database.AddRecord(DatabaseItem.GetRandom()));
        }

        private string dialogText = "Hello World!!";
        public string DialogText
        {
            get { return dialogText; }
            set { SetProperty(ref dialogText, value); }
        }

        private Theme theme;
        public Theme Theme
        {
            get { return theme; }
            set { SetProperty(ref theme, value); }
        }
        private Language lanugage;
        public Language Language
        {
            get { return lanugage; }
            set { SetProperty(ref lanugage, value, () => _statusManager.CurrentLanguage = Language); }
        }

        private bool oneWayBinding;
        public bool OneWayBinding
        {
            get { return oneWayBinding; }
            set { SetProperty(ref oneWayBinding, value); }
        }

        private bool twoWayBinding;
        public bool TwoWayBinding
        {
            get { return twoWayBinding; }
            set { SetProperty(ref twoWayBinding, value); }
        }

        private bool oneTimeBinding;
        public bool OneTimeBinding
        {
            get { return oneTimeBinding; }
            set { SetProperty(ref oneTimeBinding, value); }
        }

        private bool oneWayToSourceBinding;
        public bool OneWayToSourceBinding
        {
            get { return oneWayToSourceBinding; }
            set { SetProperty(ref oneWayToSourceBinding, value); }
        }

        private string validatingText = "Hello World!!";
        public string ValidatingText
        {
            get { return validatingText; }
            set { SetProperty(ref validatingText, value, () => UpdateValidation()); }
        }

        private DataTable data;
        public DataTable Data
        {
            get { return data; }
            set { SetProperty(ref data, value); }
        }

        private DatabaseItem dataBaseObject = DatabaseItem.GetRandom();
        public DatabaseItem DatabaseObject
        {
            get { return dataBaseObject; }
            set { SetProperty(ref dataBaseObject, value); }
        }

        public ValidatorGroup ValidationRule { get; set; }
        public ICommand ShowInfoCommand { get; set; }
        public ICommand ShowWarningCommand { get; set; }
        public ICommand ShowErrorCommand { get; set; }
        public ICommand SetBusyCommand { get; set; }
        public ICommand ShowPromptCommand { get; set; }
        public ICommand EditDatabaseCommand { get; set; }
        public ICommand LogNowCommand { get; set; }
        public IPropertyContainer SamplePropertyContainer { get; set; }
        public IPropertyContainer SamplePropertyContainerTwo { get; set; }
    }
}
