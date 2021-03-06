﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace KEI.UI.Wpf
{
    public abstract class BaseValueConverter<T> : MarkupExtension, IValueConverter
        where T: new()
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

    public abstract class BaseMultiValueConverter<T> : MarkupExtension, IMultiValueConverter
        where T : new()
    {
        public abstract object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);

        public virtual object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

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

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }
    }
}
