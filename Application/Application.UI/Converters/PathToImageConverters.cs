using KEI.UI.Wpf;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Application.UI.Converters
{
    public class PathToImageBrushConverter : BaseValueConverter<PathToImageBrushConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return null;
            else return new ImageBrush() { ImageSource = Imaging.ImageSourceFromFile(value.ToString()), Stretch = Stretch.Uniform };
        }
    }

    public class PathToImageSourceConverter : BaseValueConverter<PathToImageSourceConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return null;
            return Imaging.ImageSourceFromFile(value.ToString());
        }
    }

    public static class Imaging
    {
        public static ImageSource ImageSourceFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            var bitmap = (Bitmap)Image.FromFile(filePath);
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                                bitmap.GetHbitmap(),
                                                IntPtr.Zero,
                                                Int32Rect.Empty,
                                                BitmapSizeOptions.FromEmptyOptions());
        }

        public static ImageSource ToImageSource(this Bitmap image)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                    image.GetHbitmap(),
                                    IntPtr.Zero,
                                    Int32Rect.Empty,
                                    BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
