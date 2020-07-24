using KEI.Infrastructure.Validation;
using KEI.UI.Wpf;
using System;
using System.Globalization;

namespace Application.UI.Converters
{
    public class ValidationRuleToListConverter : ValueConverterExtension<ValidationRuleToListConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ValidatorGroup g)
                return g.Rules;
            return null;
        }
    }
}
