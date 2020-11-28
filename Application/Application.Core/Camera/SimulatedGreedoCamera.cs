using System.IO;
using KEI.Infrastructure;
using KEI.Infrastructure.Utils;
using KEI.Infrastructure.Helpers;
using KEI.Infrastructure.Service;
using KEI.Infrastructure.Configuration;

namespace Application.Core.Camera
{
    public class SimulatedCamera : ConfigHolder, IGreedoCamera
    {
        public string Name { get; set; } = "Simulated Camera";
        public bool IsConnected { get; set; }
        public override string ConfigPath => PathUtils.GetPath("Configs/camera.xcfg");
        public override string ConfigName => "CAMERA";

        private static ISystemStatusManager _systemStatusManager;
        public SimulatedCamera(ISystemStatusManager systemStatusManager, IEssentialServices essentialServices) : base(essentialServices)
        {
            _systemStatusManager = systemStatusManager;
        }
        public bool Capture(string fileName)
        {
            // TODO : Path from DataContainer
            string imagePath = string.Empty;

            string currentScreen = _systemStatusManager.CurrentScreen;
            if (currentScreen == "TeachingScreen")
            {
                imagePath = @".\Images\DummyImages\Ref.bmp";
            }
            else if (currentScreen == "TestingScreen")
            {
                imagePath = @".\Images\DummyImages\DUT3_WithGlassWindows_afterPolish.bmp";
            }
            else
            {
                imagePath = @".\Images\DummyImages\Ref.bmp";
            }

            FileHelper.Copy(imagePath, fileName);

            return true;
        }

        public bool Connect()
        {
            if (!File.Exists(@".\Images\DummyImages\Ref.bmp") ||
                !File.Exists(@".\Images\DummyImages\DUT3_WithGlassWindows_afterPolish.bmp"))
            {
                _logger.Error("Simulated Image files does not exisit");
            }

            IsConnected = true;

            return IsConnected;
        }

        public bool Disconnect()
        {
            IsConnected = false;

            return true;
        }

        public bool SetExposure(float exposure) => true;

        public bool SetPixelFormat(string pixelFormat) => true;

        public bool SetSize(int width, int height) => true;

        protected override PropertyContainerBuilder DefineConfigShape()
        {
            return PropertyContainerBuilder.Create(ConfigName, ConfigPath)
                .Property("CapturedImagesFolder", @".\Images\CapturedImages\");
        }
    }
}
