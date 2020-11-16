using KEI.Infrastructure.Service;
using KEI.UI.Wpf;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace ServiceEditor.Converters
{
    public class ServiceToHasConfigConverter : BaseValueConverter<ServiceToHasConfigConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (File.Exists("ConfigEditor.exe") == false)
            {
                return false;
            }

            if (value is ServiceInfo s)
            {
                if (s.ImplementationType.GetUnderlyingType() is Type t)
                {
                    var obj = FormatterServices.GetUninitializedObject(t);
                    return obj is IConfigurable;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }
    }
}
