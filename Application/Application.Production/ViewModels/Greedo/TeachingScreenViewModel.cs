using System;
using System.IO;
using System.Threading.Tasks;
using Prism.Commands;
using KEI.Infrastructure;
using KEI.Infrastructure.Configuration;
using KEI.Infrastructure.Prism;
using KEI.Infrastructure.Screen;
using KEI.UI.Wpf;
using KEI.UI.Wpf.Hotkey;
using Application.Core;
using Application.Core.Camera;
using Application.Core.Constants;
using Application.Core.Interfaces;
using Application.Production.Views;

namespace Application.Production.ViewModels
{
    [Screen(DisplayName = ScreenDisplayNames.TeachingScreen,
            Icon = Icon.TestConfiguration16x,
            ScreenName = nameof(TeachingScreen))]
    public class TeachingScreenViewModel : BaseScreenViewModel<TeachingScreen>
    {
        #region BaseScreenViewModel Members
        public override string DisplayName { get; set; } = "Teaching";
        public override Icon Icon { get; set; } = Icon.TestConfiguration16x;

        #endregion

        #region Injected Instances

        private readonly IGreedoCamera _camera;
        private readonly IVisionProcessor _processor;
        private readonly IPropertyContainer _camerConfig;
        private readonly IViewService _viewService;

        #endregion

        #region Constructor
        public TeachingScreenViewModel(IGreedoCamera camera, IVisionProcessor vision, IConfigManager configManager, 
            IViewService viewService, IHotkeyService hotkeyService) : base(hotkeyService)
        {
            _camera = camera;
            _processor = vision;
            _camerConfig = configManager.GetConfig(ConfigKeys.Camera);
            _viewService = viewService;

            Geometries = XmlHelper.Deserialize<ShapeGeometryCollection>("Configs/reference.cfg");

            Commands.Add(new Screen.ScreenCommand
            {
                DisplayName = "Change Theme",
                Command = UpdateTheme,
                Icon = Icon.EditTheme16x
            });

            var gp = configManager.GetConfig(ConfigKeys.GeneralPreferences);
            gp.SetBinding("Theme", () => CurrentTheme, BindingMode.OneWayToSource);
        }

        #endregion

        private DelegateCommand updateTheme;
        public DelegateCommand UpdateTheme =>
            updateTheme ?? (updateTheme = new DelegateCommand(ExecuteUpdateTheme));

        void ExecuteUpdateTheme()
        {
            CurrentTheme = (Theme)((int)CurrentTheme + 1);
        }

        private Theme currentTheme;
        public Theme CurrentTheme
        {
            get { return currentTheme; }
            set { SetProperty(ref currentTheme, value); }
        }

        #region Properties

        private string imagePath;
        public string ImagePath
        {
            get => imagePath;
            set => SetProperty(ref imagePath, value);
        }

        private bool showObjects;
        public bool ShowObjects
        {
            get => showObjects;
            set => SetProperty(ref showObjects, value);
        }
        public ShapeGeometryCollection Geometries { get; set; }

        #endregion

        #region Save ROI Command

        private DelegateCommand saveROICommand;
        public DelegateCommand SaveROICommand =>
            saveROICommand ?? (saveROICommand = new DelegateCommand(ExecuteSaveROICommand));
        void ExecuteSaveROICommand() => XmlHelper.Serialize(XmlHelper.Serialize(Geometries, "Configs/reference.cfg"));

        #endregion

        #region Inspect Command

        private DelegateCommand inspectCommand;
        public DelegateCommand InspectCommand =>
            inspectCommand ?? (inspectCommand = new DelegateCommand(ExecuteInspectCommand));
        private async void ExecuteInspectCommand()
        {
            ExecuteCaptureCommand();

            await Task.Delay(500);

            var result = _processor.ReferenceTest(ImagePath);

            ImagePath = @".\Images\DummyImages\1_Corrected.bmp";

            ShowObjects = true;
        }

        #endregion

        #region Capture Command

        private DelegateCommand captureCommand;
        public DelegateCommand CaptureCommand =>
            captureCommand ?? (captureCommand = new DelegateCommand(ExecuteCaptureCommand));
        private void ExecuteCaptureCommand()
        {
            ShowObjects = false;
            var path = GenerateImagePath();
            _camera.Capture(path);
            ImagePath = path;
        }

        #endregion

        #region Save Background Command

        private DelegateCommand saveBGCommand;
        public DelegateCommand SaveBGCommand =>
            saveBGCommand ?? (saveBGCommand = new DelegateCommand(ExecuteSaveBGCommand));

        void ExecuteSaveBGCommand() => _viewService.Prompt("Do you want to save background image ?", PromptOptions.YesNo);

        #endregion

        #region Private Functions
        private string GenerateImagePath()
        {
            string imageName = $"{DateTime.Now.ToString("[dd-MMM-yyyy] hh_mm_ss")}.bmp";
            string imageDir = string.Empty;
            _camerConfig.Get(ConfigKeys.CameraConfigKeys.CapturedImagesFolder, ref imageDir);

            return Path.Combine(imageDir, imageName);
        }

        #endregion

    }
}
