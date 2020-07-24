using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace KEI.UI.Wpf
{
    public abstract class ValueConverterExtension<T> : MarkupExtension, IValueConverter where T: new()
    {
        private T instance = default(T);
        public T Instance
        {
            get
            {
                if (instance == null)
                    instance = new T();
                return instance;
            }
        }

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }
    }
}
