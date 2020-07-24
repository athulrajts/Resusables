using System;
using System.Windows;
using System.Reflection;
using System.ComponentModel;
using KEI.Infrastructure.Helpers;

namespace KEI.Infrastructure.Configuration
{
    /// <summary>
    /// Object Representing a binding between a CLR Property
    /// and a <see cref="PropertyObject"/>. CLR Property should invoke
    /// <see cref="PropertyChangedEventHandler"/> in property setter for the binding to work
    /// </summary>
    internal class PropertyBinding : IDisposable
    {
        public PropertyBinding(object target, DataObject property, PropertyInfo targetProperty, BindingMode mode)
        {
            BindingTarget = new WeakReference(target);
            Property = property;
            TargetProperty = targetProperty;
            Mode = mode;

            UpdateTarget();

            if (Mode == BindingMode.OneTime)
                return;

            if (BindingTarget.Target is INotifyPropertyChanged inpc &&
                (Mode == BindingMode.TwoWay || Mode == BindingMode.OneWayToSource))
            {
                inpc.PropertyChanged += OnPropertyChangeRaised;
            }

            if (Mode != BindingMode.OneWayToSource)
            {
                Property.PropertyChanged += OnPropertyChangeRaised;
            }
        }

        private void OnPropertyChangeRaised(object sender, PropertyChangedEventArgs e)
        {
            if (sender == BindingTarget.Target)
            {
                if (e.PropertyName == TargetProperty.Name)
                {
                    if (BindingTarget.IsAlive)
                    {
                        Property.Value = TargetProperty.GetValue(BindingTarget.Target);
                    }
                }
            }
            else if (sender == Property)
            {
                if (e.PropertyName == Property.Name)
                {
                    UpdateTarget();
                }
            }
        }

        /// <summary>
        /// Referece to <see cref="DataObject"/> or more derrived classes.
        /// </summary>
        public DataObject Property { get; set; }

        /// <summary>
        /// Reference to CLR Property
        /// </summary>
        public PropertyInfo TargetProperty { get; set; }

        /// <summary>
        /// reference to owner of CLR Property
        /// keeping a weak reference to owner to allow it to be 
        /// garbage collected even when this object is alive
        /// </summary>
        public WeakReference BindingTarget { get; set; }

        /// <summary>
        /// Specifies the binding mode
        /// </summary>
        public BindingMode Mode { get; set; }

        /// <summary>
        /// Update value of CLR Property
        /// </summary>
        /// <param name="newValue"></param>
        private void UpdateTarget()
        {
            if (TargetProperty?.CanWrite == true)
            {
                object value = null;
                if (Property.Value is Selector s)
                {
                    var type = s.Type.GetUnderlyingType();
                    if (type.IsEnum)
                    {
                        value = Enum.Parse(type, s.SelectedItem);
                    }
                    else
                    {
                        value = type.ConvertFrom(s.SelectedItem);
                    }
                }
                else if (Property.Value is IConvertible)
                {
                    value = TargetProperty.PropertyType.ConvertFrom(Property.ValueString);
                }

                // Update the target property if the owner hasn't already been GC'ed
                if (BindingTarget.IsAlive)
                {
                    TargetProperty.SetValue(BindingTarget.Target, value); 
                }
            }
        }

        #region IDisposible Members

        public void Dispose()
        {
            if (Mode == BindingMode.OneTime)
            {
                BindingTarget = null;
                return;
            }

            if (BindingTarget.Target is INotifyPropertyChanged inpc &&
                (Mode == BindingMode.TwoWay || Mode == BindingMode.OneWayToSource))
            {
                inpc.PropertyChanged -= OnPropertyChangeRaised;
            }

            if (Mode != BindingMode.OneWayToSource)
            {
                Property.PropertyChanged -= OnPropertyChangeRaised;
            }

            BindingTarget = null;
        }

        #endregion
    }
}
