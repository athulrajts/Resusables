using System;
using System.Globalization;
using KEI.Infrastructure.Types;
using KEI.UI.Wpf;

namespace ServiceEditor.Converters
{
    public class TypeToImplementationsConverter : BaseValueConverter<TypeToImplementationsConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is TypeInfo t
                ? ImplementationsProvider.Instance.GetImplementations(t.GetUnderlyingType())
                : null;

        }
    }
}
