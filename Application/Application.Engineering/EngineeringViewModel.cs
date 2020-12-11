using System.IO;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using Prism.Events;
using Prism.Commands;
using KEI.Infrastructure;
using KEI.Infrastructure.Prism;
using KEI.Infrastructure.Utils;
using KEI.Infrastructure.Events;
using KEI.Infrastructure.Database;
using KEI.UI.Wpf.Hotkey;
using Application.Core;
using Application.Core.Interfaces;
using Application.UI.Controls;
using Application.UI.Converters;
using Application.Engineering.Layout;
using Application.Engineering.Dialogs;

namespace Application.Engineering
{
    [RegisterSingleton(NeedResolve = false)]
    public class EngineeringViewModel : BindableBase
    {
        private readonly ISystemStatusManager _statusManager;
        private readonly IApplicationViewService _viewService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IEquipment _equipment;
        private readonly IDatabaseManager _databaseManager;
        private readonly IHotkeyService _hotkeyManager;
        private bool[] viewMemory;


        public EngineeringViewModel(ISystemStatusManager statusManager, IApplicationViewService viewService, IEventAggregator eventAggregator,
            IEquipment equipment, IDatabaseManager databaseManager, IHotkeyService hotkeyManager)
        {
            _statusManager = statusManager;
            _viewService = viewService;
            _eventAggregator = eventAggregator;
            _equipment = equipment;
            _databaseManager = databaseManager;
            _hotkeyManager = hotkeyManager;

            SubscribeToEvents();

            var configPath = PathUtils.GetPath(@"Configs/view.view");

            Sections = XmlHelper.DeserializeFromFile<ObservableCollection<ContentSectionModel>>(configPath);

            if(Sections is null)
            {
                Sections = GetDefaultSections();
                XmlHelper.SerializeToFile(Sections, configPath);
            }

            viewMemory = Sections.Select(x => x.IsChecked).ToArray();
        }

        private ObservableCollection<ContentSectionModel> GetDefaultSections()
        {
            return new ObservableCollection<ContentSectionModel>
            {
                new ContentSectionModel
                {
                    Region = "Result",
                    Title = "Result",
                    IconSource = "Resources/results.png",
                    Dock = System.Windows.Controls.Dock.Right,
                    MinWidth = 100,
                    IsChecked = true
                },
                new ContentSectionModel
                {
                    Region = "Database",
                    Title = "Database",
                    IconSource = "Resources/database.png",
                    Dock = System.Windows.Controls.Dock.Right,
                    MinWidth = 300,
                    IsChecked = true
                },
                new ContentSectionModel
                {
                    Region = "Image",
                    Title = "Image",
                    IconSource = "Resources/image.png",
                    Dock = System.Windows.Controls.Dock.Left,
                    IsChecked = true
                }

            };
        }

        private void SubscribeToEvents()
        {
            _eventAggregator.GetEvent<SectionExpanded>().Subscribe(section =>
            {
                viewMemory = Sections.Select(x => x.IsChecked).ToArray();

                foreach (var uiSection in Sections.Where(x => x.IsChecked))
                {
                    if (uiSection != section)
                        uiSection.IsChecked = false;
                }
            });

            _eventAggregator.GetEvent<SectionMinimized>().Subscribe(section =>
            {
                for (int i = 0; i < viewMemory.Length; i++)
                {
                    Sections[i].IsChecked = viewMemory[i];
                }
            });

            _eventAggregator.GetEvent<RecipeLoadedEvent>().Subscribe(rcp => 
            {
                RaisePropertyChanged(nameof(CurrentRecipePath));
            });

        }

        private ObservableCollection<ContentSectionModel> sections = new ObservableCollection<ContentSectionModel>();
        public ObservableCollection<ContentSectionModel> Sections
        {
            get { return sections; }
            set { SetProperty(ref sections, value); }
        }

        public bool HasLogViewer { get; set; } = File.Exists("YALV.exe");

        public string CurrentRecipePath => string.IsNullOrEmpty(_equipment.CurrentRecipe?.FilePath)
            ? string.Empty
            : Path.GetFullPath(_equipment.CurrentRecipe.FilePath);

        #region Switch To Production

        private DelegateCommand switchToProductionModeCommand;
        public DelegateCommand SwitchToProductionModeCommand =>
            switchToProductionModeCommand ?? (switchToProductionModeCommand = new DelegateCommand(ExecuteSwitchToProductionModeCommand));

        void ExecuteSwitchToProductionModeCommand() => _statusManager.ApplicationMode = ApplicationMode.Production;

        #endregion

        #region Start Test

        private DelegateCommand startTest;
        public DelegateCommand StartTestCommand =>
            startTest ?? (startTest = new DelegateCommand(ExecuteStartTestCommand));

        void ExecuteStartTestCommand()
        {
            _databaseManager[$"{ApplicationMode.Engineering} DB"].StartSession(_equipment.CurrentDatabaseSetup.Morph() as DatabaseSetup);

            _equipment.ExecuteTest();
        }

        #endregion

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

        #region Advanced Setup

        private DelegateCommand showAdvancedSetup;
        public DelegateCommand ShowAdvancedSetupCommand =>
            showAdvancedSetup ?? (showAdvancedSetup = new DelegateCommand(ExecuteShowAdvancedSetupCommand));

        void ExecuteShowAdvancedSetupCommand() => _viewService.ShowAdvancedSetup();

        #endregion

        #region Layout Setup

        private DelegateCommand<object> editObjectCommand;
        public DelegateCommand<object> EditObjectCommand =>
            editObjectCommand ?? (editObjectCommand = new DelegateCommand<object>(ExecuteEditObjectCommand));

        void ExecuteEditObjectCommand(object param) => _viewService.EditObject(param);

        #endregion

        #region Open Recipe

        private DelegateCommand openRecipeCommand;
        public DelegateCommand OpenRecipeCommand =>
            openRecipeCommand ?? (openRecipeCommand = new DelegateCommand(ExecuteOpenRecipeCommand));

        void ExecuteOpenRecipeCommand()
        {
            _statusManager.CurrentLanguage = Language.English;

            var file = _viewService.BrowseFile(new FilterCollection { new Filter("Recipe Files", ".rcp") });

            if (string.IsNullOrEmpty(file))
                return;

            _equipment.LoadRecipe(file);
        }

        #endregion

        #region Open Layout

        private DelegateCommand openLayoutCommand;
        public DelegateCommand OpenLayoutCommand =>
            openLayoutCommand ?? (openLayoutCommand = new DelegateCommand(ExecuteOpenLayoutCommand));

        void ExecuteOpenLayoutCommand()
        {
            var file = _viewService.BrowseFile(new FilterCollection { new Filter("Layout Files", ".view") });

            if (string.IsNullOrEmpty(file))
                return;

            if(XmlHelper.DeserializeFromFile<ObservableCollection<ContentSectionModel>>(file) is ObservableCollection<ContentSectionModel> layout)
            {
                Sections = layout;
            }
        }

        #endregion

        #region Save Recipe

        private DelegateCommand saveRecipeCommand;
        public DelegateCommand SaveRecipeCommand =>
            saveRecipeCommand ?? (saveRecipeCommand = new DelegateCommand(ExecuteSaveRecipeCommand));

        void ExecuteSaveRecipeCommand()
            => _viewService.SaveFile((file) => _equipment.StoreRecipe(file), "Recipe File | *.rcp");

        #endregion

        #region Save Layout


        private DelegateCommand saveLayoutCommand;
        public DelegateCommand SaveLayoutCommand =>
            saveLayoutCommand ?? (saveLayoutCommand = new DelegateCommand(ExecuteSaveLayoutCommand));

        void ExecuteSaveLayoutCommand()
            => _viewService.SaveFile((file) => XmlHelper.SerializeToString(Sections), "Layout file | *.view");

        #endregion

        #region Open Folder

        private DelegateCommand<string> openFolderCommnd;
        public DelegateCommand<string> OpenFolderCommand =>
            openFolderCommnd ?? (openFolderCommnd = new DelegateCommand<string>(ExecuteOpenFolderCommand));

        void ExecuteOpenFolderCommand(string parameter)
        {
            if(string.IsNullOrEmpty(parameter))
                return;

            string folderPath = string.Empty;

            switch(parameter)
            {
                case "Database":
                    folderPath = @"Database/Engineering/";
                    break;
                case "Logs":
                    folderPath = @"Logs/";
                    break;
                case "Images":
                    folderPath = $"Images/CapturedImages/";
                    break;
                default:
                    var attr = File.GetAttributes(parameter);
                    if (attr.HasFlag(FileAttributes.Directory))
                        folderPath = parameter;
                    else
                        folderPath = Path.GetDirectoryName(parameter);
                    break;
            }

            if (Directory.Exists(folderPath))
                Process.Start("explorer", Path.GetFullPath(folderPath));
        }

        #endregion

        #region Open Log File

        private DelegateCommand openLogFileCommand;     
        public DelegateCommand OpenLogFileCommand =>
            openLogFileCommand ?? (openLogFileCommand = new DelegateCommand(ExecuteOpenLogFileCommand));

        void ExecuteOpenLogFileCommand()
        {
            string logPath = @"Logs/log.xlog";
            if (File.Exists(logPath))
                Process.Start(Path.GetFullPath("YALV.exe"), logPath);
        }

        #endregion

        #region Open Database

        private DelegateCommand openDatabaseCommand;
        public DelegateCommand OpenDatabaseCommand =>
            openDatabaseCommand ?? (openDatabaseCommand = new DelegateCommand(ExecuteOpenDatabaseCommand));

        void ExecuteOpenDatabaseCommand()
        {
            var file = _viewService.BrowseFile(new FilterCollection { new Filter("Database Files", ".csv") });

            if (string.IsNullOrEmpty(file))
                return;

            var window = new Window
            {
                Title = file,
                Icon = Properties.Resources.database_img.ToImageSource(),
                Content = new DatabaseViewer { Data = _databaseManager.GetData(file) },
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            window.Show();
        }

        #endregion

        #region Change Lanugage

        private DelegateCommand changeLanguageCommand;
        public DelegateCommand ChangeLanugageCommand =>
            changeLanguageCommand ?? (changeLanguageCommand = new DelegateCommand(ExecuteChangeLanugageCommand));

        void ExecuteChangeLanugageCommand() => ChangeLanguage.Prompt();

        #endregion
    }
}
