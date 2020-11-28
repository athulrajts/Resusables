using System;
using System.IO;
using System.Collections.Generic;
using Prism.Mvvm;
using Prism.Events;
using KEI.Infrastructure;
using KEI.Infrastructure.Database;
using KEI.Infrastructure.Configuration;
using Application.Core.Camera;
using Application.Core.Constants;
using Application.Core.Interfaces;
using KEI.Infrastructure.Events;
using KEI.Infrastructure.Utils;

namespace Application.Core.Modules
{
    public class Equipment : BindableBase, IEquipment
    {
        private readonly IApplicationViewService _viewService;
        private readonly IConfigManager _configManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IGreedoCamera _camera;
        private readonly ISystemStatusManager _statusManager;
        private readonly IVisionProcessor _processor;

        private readonly PropertyContainerBuilder _defaultRecipe;
        private readonly IPropertyContainer _cameraConfig;
        public Equipment(IEventAggregator ea, IDatabaseManager dbm, IApplicationViewService viewService,
            IConfigManager configManager, IVisionProcessor processor, IGreedoCamera camera,
            ISystemStatusManager statusManager)
        {
            _eventAggregator = ea;
            _camera = camera;
            _configManager = configManager;
            _databaseManager = dbm;
            _processor = processor;
            _statusManager = statusManager;
            _viewService = viewService;

            _cameraConfig = _configManager.GetConfig(ConfigKeys.Camera);

            _defaultRecipe = PropertyContainerBuilder.Create("Recipe", PathUtils.GetPath("Configs/DefaultRecipe.rcp"))
                .Property("MaximumTransmittance", 100.0)
                .Property("MinimumTransmittance", 90.0)
                .Property("Production DB", new DatabaseSetup { Schema = new List<DatabaseColumn>(DatabaseSchema.SchemaFor<TestResult>()), Name = PathUtils.GetPath("Database/Production/Production.csv"), CreationMode = DatabaseCreationMode.Daily })
                .Property("Engineering DB", new DatabaseSetup { Schema = new List<DatabaseColumn>(DatabaseSchema.SchemaFor<TestResult>()), Name = PathUtils.GetPath("Database/Engineering/Engineering.csv"), CreationMode = DatabaseCreationMode.Daily });

            //        _defaultRecipe = PropertyContainerBuilder.Create("Recipe", PathUtils.GetPath("Configs/DefaultRecipe.rcp"))
            //.Property("MaximumTransmittance", 100.0, "Maximum Value of Transmittance allowed that considered pass")
            //.Property("MinimumTransmittance", 90.0, "Maximum Value of Transmittance allowed that considered pass")
            //.Property("Production DB", new DatabaseSetup { Schema = new List<DatabaseColumn>(DatabaseSchema.SchemaFor<TestResult>()), Name = PathUtils.GetPath("Database/Production/Production.csv"), CreationMode = DatabaseCreationMode.Daily })
            //.Property("Engineering DB", new DatabaseSetup { Schema = new List<DatabaseColumn>(DatabaseSchema.SchemaFor<TestResult>()), Name = PathUtils.GetPath("Database/Engineering/Engineering.csv"), CreationMode = DatabaseCreationMode.Daily });
        }

        private IPropertyContainer currentRecipe;
        public IPropertyContainer CurrentRecipe
        {
            get { return currentRecipe; }
            private set { SetProperty(ref currentRecipe, value, OnRecipeChanged); }
        }

        private IDataContainer engineeringDBSetup;
        private IDataContainer productionDBSetup;
        public IDataContainer CurrentDatabaseSetup
        {
            get
            {
                return _statusManager.ApplicationMode == ApplicationMode.Production
                     ? productionDBSetup
                     : engineeringDBSetup;
            }
        }

        private void OnRecipeChanged()
        {
            CurrentRecipe.GetValue($"{ApplicationMode.Production} DB", ref productionDBSetup);

            var xml = XmlHelper.Serialize(productionDBSetup);
            CurrentRecipe.GetValue($"{ApplicationMode.Engineering} DB", ref engineeringDBSetup);

            RecipeLoaded?.Invoke(this, CurrentRecipe);
            _eventAggregator.GetEvent<RecipeLoadedEvent>().Publish(CurrentRecipe);
        }

        public void LoadRecipe(string path)
        {
            CurrentRecipe = PropertyContainerBuilder.FromFile(path);

            if (CurrentRecipe == null)
            {
                RestoreDefaultRecipe();

                CurrentRecipe.GetValue($"{ApplicationMode.Production} DB", ref productionDBSetup);
                CurrentRecipe.GetValue($"{ApplicationMode.Engineering} DB", ref engineeringDBSetup);

                CurrentRecipe.Store();
            }
        }

        public void StoreRecipe(string path)
        {
            CurrentRecipe.Store(path);
        }

        public void RestoreDefaultRecipe() => CurrentRecipe = _defaultRecipe.Build();

        public event EventHandler<IPropertyContainer> RecipeLoaded;

        public void ExecuteTest()
        {
            if (_viewService.StartTestDialog() == PromptResult.Cancel)
            {
                return;
            }


            string imageName = $"{DateTime.Now.ToString("[dd-MMM-yyyy] hh_mm_ss")}.bmp";
            string imageDir = string.Empty;
            _cameraConfig.GetValue(ConfigKeys.CameraConfigKeys.CapturedImagesFolder, ref imageDir);

            string image = Path.Combine(imageDir, imageName);

            _camera.Capture(image);

            var result = _processor.Test(image);

            result.ForEach(x => _databaseManager[$"{_statusManager.ApplicationMode} DB"].AddRecord(x));

            _eventAggregator.GetEvent<TestExecuted>().Publish(new Tuple<ApplicationMode, string, List<TestResult>>(_statusManager.ApplicationMode, image, result));
        }
    }
}
