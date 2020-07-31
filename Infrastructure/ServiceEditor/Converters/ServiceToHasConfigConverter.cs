using KEI.Infrastructure.Service;
using KEI.UI.Wpf;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace ServiceEditor.Converters
{
    public class ServiceToHasConfigConverter : ValueConverterExtension<ServiceToHasConfigConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (File.Exists("ConfigEditor.exe") == false)
            {
                return false;
            }

            if (value is Service s)
            {
                if (s.ImplementationType.GetUnderlyingType() is Type t)
                {
                    if (t.GetProperty("ConfigPath") is PropertyInfo cpi)
                    {
                        var obj = FormatterServices.GetUninitializedObject(t);
                        var path = cpi.GetValue(obj)?.ToString();

                        if(File.Exists(path) == false)
                        {
                            if (t.GetMethod("DefineConfigShape", BindingFlags.NonPublic | BindingFlags.Instance) is MethodInfo mi)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
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
