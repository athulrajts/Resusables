using System;
using System.Reflection;
using System.ComponentModel;

namespace KEI.Infrastructure
{
    /// <summary>
    /// Object Representing a binding between a CLR Property
    /// and a <see cref="PropertyObject"/>. CLR Property should invoke
    /// <see cref="PropertyChangedEventHandler"/> in property setter for the binding to work
    /// </summary>
    internal class DataObjectBinding : IDisposable
    {
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
        /// Constructor
        /// </summary>
        /// <param name="target"></param>
        /// <param name="property"></param>
        /// <param name="targetProperty"></param>
        /// <param name="mode"></param>
        public DataObjectBinding(object target, DataObject property, PropertyInfo targetProperty, BindingMode mode)
        {
            BindingTarget = new WeakReference(target);
            Property = property;
            TargetProperty = targetProperty;
            Mode = mode;

            UpdateTarget();

            if (Mode == BindingMode.OneTime)
            {
                return;
            }

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

        /// <summary>
        /// Update Target / Source on property change events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPropertyChangeRaised(object sender, PropertyChangedEventArgs e)
        {
            if (sender == BindingTarget.Target)
            {
                if (e.PropertyName == TargetProperty.Name)
                {
                    if (BindingTarget.IsAlive)
                    {
                        Property.SetValue(TargetProperty.GetValue(BindingTarget.Target));
                    }
                }
            }
            else if (sender == Property)
            {
                if (e.PropertyName == "Value")
                {
                    UpdateTarget();
                }
            }
        }

        /// <summary>
        /// Update value of CLR Property
        /// </summary>
        /// <param name="newValue"></param>
        private void UpdateTarget()
        {
            if (TargetProperty?.CanWrite == true)
            {
                // Update the target property if the owner hasn't already been GC'ed
                if (BindingTarget.IsAlive)
                {
                    TargetProperty.SetValue(BindingTarget.Target, Property.GetValue()); 
                }
            }
        }

        #region IDisposible Members

        /// <summary>
        /// Implementation for <see cref="IDisposable.Dispose"/>
        /// </summary>
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
