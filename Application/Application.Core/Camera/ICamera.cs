using KEI.Infrastructure.Service;

namespace Application.Core.Camera
{
    [Service("Camera")]
    public interface IGreedoCamera : ICamera, ICanSetExposure, ICanSetPixelFormat, ICanSetSize { }

    public interface ICamera
    {
        bool IsConnected { get; set; }
        string Name { get; set; }
        bool Connect();
        bool Disconnect();
        bool Capture(string imagePath);
    }

    public interface ICanSetExposure
    {
        bool SetExposure(float exposure);
    }

    public interface ICanSetPixelFormat
    {
        bool SetPixelFormat(string pixelFormat);
    }

    public interface ICanSetSize
    {
        bool SetSize(int width, int height);
    }

    public interface ICanSetGamma
    {
        bool SetGamma(bool enable);
        bool SetGammaValue(float gammaValue);
    }

    public interface ICanSetGain
    {
        bool SetGain(long gain);
    }
}
