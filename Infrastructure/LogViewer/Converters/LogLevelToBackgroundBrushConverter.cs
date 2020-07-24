using KEI.Infrastructure.Logging;
using KEI.UI.Wpf;
using System;
using System.Globalization;
using System.Windows.Media;

namespace LogViewer.Converters
{
    public class LogLevelToBackgroundBrushConverter : ValueConverterExtension<LogLevelToBackgroundBrushConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is LogLevel e)
            {
                return e switch
                {
                    LogLevel.Debug => new SolidColorBrush(Colors.White),
                    LogLevel.Info => new SolidColorBrush(Colors.LightGreen),
                    LogLevel.Warn => new SolidColorBrush(Colors.Yellow),
                    LogLevel.Error => new SolidColorBrush(Colors.OrangeRed),
                    LogLevel.Fatal => new SolidColorBrush(Colors.Red),
                    _ => new SolidColorBrush(Colors.White)
                };
            }

            return Brushes.Transparent;
        }
    }
}
