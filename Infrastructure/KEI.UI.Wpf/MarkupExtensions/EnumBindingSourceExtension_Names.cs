using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace KEI.UI.Wpf
{
    public class EnumNameBindingSourceExtension : MarkupExtension
    {
        private readonly BindingBase _binding;
        private Type _enumType;
        public Type EnumType
        {
            get { return this._enumType; }
            set
            {
                if (value != this._enumType)
                {
                    if (null != value)
                    {
                        Type enumType = Nullable.GetUnderlyingType(value) ?? value;
                        if (!enumType.IsEnum)
                            return;
                    }

                    this._enumType = value;
                }
            }
        }

        public EnumNameBindingSourceExtension() { }

        public EnumNameBindingSourceExtension(Type enumType)
        {
            EnumType = enumType;
        }

        public EnumNameBindingSourceExtension(BindingBase binding)
        {
            _binding = binding;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_binding != null)
            {
                var pvt = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
                var target = pvt.TargetObject as DependencyObject;

                // if we are inside a template, WPF will call us again when it is applied
                if (target == null)
                    return this;

                BindingOperations.SetBinding(target, EnumTypeProperty, _binding);
                EnumType = (Type)target.GetValue(EnumTypeProperty);
                BindingOperations.ClearBinding(target, EnumTypeProperty);
            }

            if (null == _enumType)
                return null;

            Type actualEnumType = Nullable.GetUnderlyingType(this._enumType) ?? this._enumType;

            var enumValues = Enum.GetNames(actualEnumType);

            if (actualEnumType == this._enumType)
                return enumValues;

            Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            enumValues.CopyTo(tempArray, 1);
            return tempArray;
        }




        public static Type GetEnumType(DependencyObject obj)
        {
            return (Type)obj.GetValue(EnumTypeProperty);
        }

        public static void SetEnumType(DependencyObject obj, Type value)
        {
            obj.SetValue(EnumTypeProperty, value);
        }

        public static readonly DependencyProperty EnumTypeProperty =
            DependencyProperty.RegisterAttached("EnumType", typeof(Type), typeof(EnumNameBindingSourceExtension), new PropertyMetadata(null));


    }
}
