using System;
using System.Collections;
using System.Globalization;
using KEI.Infrastructure.Configuration;

namespace KEI.UI.Wpf.Converters
{
    /// <summary>
    /// Taskes in <see cref="object"/>
    /// Returns <see cref="object.ToString" of <see cref="object.GetType()"/>
    /// If Object is Generic type, returns in readable format
    /// </summary>
    public class ObjectToNameConverter : BaseValueConverter<ObjectToNameConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var type = value.GetType();
                if (value is IList l)
                {
                    return type.GenericTypeArguments.Length > 0 ? $"List<{type.GenericTypeArguments[0].Name}>" : "List";
                }
                else if(value is Selector s)
                {
                    return s.Type.Name;
                }
                else if(value is DataContainerBase dbs && dbs.UnderlyingType != null)
                {
                    return dbs.UnderlyingType.Name;
                }
                else
                {
                    return type.GenericTypeArguments.Length > 0 ? $"{value.GetType().Name.Split('`')[0]}<{type.GenericTypeArguments[0].Name}>" : value.GetType().Name;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Taskes in <see cref="Type"/>
    /// Returns <see cref="object.ToString" of <see cref="object.GetType()"/>
    /// If it is a Generic type, returns in readable format
    /// </summary>
    public class TypeToNameConverter : BaseValueConverter<TypeToNameConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Type t)
            {
                if (typeof(IList).IsAssignableFrom(t))
                {
                    return t.GenericTypeArguments.Length > 0 ? $"List<{t.GenericTypeArguments[0].Name}>" : "List";
                }
                else
                {
                    return t.GenericTypeArguments.Length > 0 ? $"{t.Name.Split('`')[0]}<{t.GenericTypeArguments[0].Name}>" : t.Name;
                }
            }

            return null;
        }
    }
}
