using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using KEI.Infrastructure.Localizer;

namespace KEI.UI.Wpf
{
    public class Localize : MarkupExtension
    {
        public string Value { get; set; }

        public string Localizer { get; set; } = "Localizer";

        private readonly BindingBase _binding;


        public Localize(BindingBase binding)
        {
            _binding = binding;
        }

        public Localize()
        {

        }

        public Localize(string value)
        {
            Value = value;
        }

        public override object ProvideValue(IServiceProvider provider)
        {
            object source = null;

            if (!TryGetTargetItems(provider, out DependencyObject target, out DependencyProperty prop))
            {
                // if we are inside a template, WPF will call us again when it is applied
                return this;
            }

            if (_binding != null)
            {
                BindingOperations.SetBinding(target, ValueProperty, _binding);
                Value = (string)target.GetValue(ValueProperty);
                BindingOperations.ClearBinding(target, ValueProperty);
            }


            if(target.GetValue(FrameworkElement.DataContextProperty) is object o)
            {
                source = LocalizationManager.Instance[o.GetType().Assembly.GetName().Name];
            }
            else if(target.GetValue(FrameworkContentElement.DataContextProperty) is object o2)
            {
                source = LocalizationManager.Instance[o2.GetType().Assembly.GetName().Name];
            }
            else
            {
                var parentUC = GetParentUserControl(target);
                if(parentUC != null)
                {
                    source = LocalizationManager.Instance[parentUC.GetType().Assembly.GetName().Name];
                }
            }

            var binding = new Binding
            {
                Path = new PropertyPath($"[{Value}]"),
                FallbackValue = Value,
                Mode = BindingMode.OneWay
            };

            if(source == null)
            {
                binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1);
                binding.Path = new PropertyPath($"DataContext.{Localizer}[{Value}]");
            }
            else
            {
                binding.Source = source;
            }

            BindingOperations.SetBinding(target, prop, binding);

            return binding.ProvideValue(provider);
        }

        private UserControl GetParentUserControl(DependencyObject obj)
        {
            if(obj == null)
            {
                return null;
            }

            DependencyObject parent = LogicalTreeHelper.GetParent(obj);

            if(parent is UserControl uc)
            {
                return uc;
            }
            else
            {
                return GetParentUserControl(parent);
            }

        }

        protected virtual bool TryGetTargetItems(IServiceProvider provider, out DependencyObject target, out DependencyProperty dp)
        {
            target = null;
            dp = null;
            if (provider == null) return false;

            //create a binding and assign it to the target
            IProvideValueTarget service = (IProvideValueTarget)provider.GetService(typeof(IProvideValueTarget));
            if (service == null) return false;

            //we need dependency objects / properties
            target = service.TargetObject as DependencyObject;
            dp = service.TargetProperty as DependencyProperty;
            return target != null && dp != null;
        }



        public static string GetValue(DependencyObject obj)
        {
            return (string)obj.GetValue(ValueProperty);
        }

        public static void SetValue(DependencyObject obj, string value)
        {
            obj.SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.RegisterAttached("Value", typeof(string), typeof(Localize), new PropertyMetadata(""));




    }
}
