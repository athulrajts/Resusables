using System;
using System.ComponentModel;

namespace KEI.Infrastructure
{
    /// <summary>
    /// an implementation for <see cref="PropertyDescriptor"/> based on <see cref="DataObject"/>
    /// </summary>
    public class DataObjectPropertyDescriptor : PropertyDescriptor
    {
        public DataObject DataObject { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="attributes"></param>
        internal DataObjectPropertyDescriptor(DataObject obj, Attribute[] attributes)
            : base(obj.Name, attributes)
        {
            DataObject = obj;
        }

        /// <summary>
        /// Implementation for <see cref="PropertyDescriptor.PropertyType"/>
        /// </summary>
        public override Type PropertyType
        {
            get { return DataObject.GetDataType(); }
        }

        /// <summary>
        /// Implementation for <see cref="PropertyDescriptor.SetValue(object, object)"/>
        /// </summary>
        /// <param name="component"></param>
        /// <param name="value"></param>
        public override void SetValue(object component, object value)
        {
            DataObject.SetValue(value);
        }

        /// <summary>
        /// Implementation for <see cref="PropertyDescriptor.GetValue(object)"/>
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override object GetValue(object component)
        {
            return DataObject.GetValue();
        }

        /// <summary>
        /// Implementation for <see cref="PropertyDescriptor.IsReadOnly"/>
        /// </summary>
        public override bool IsReadOnly
        {
            get
            {
                return DataObject is PropertyObject po && po.BrowseOption == BrowseOptions.NonEditable;
            }
        }

        /// <summary>
        /// Implementation for <see cref="PropertyDescriptor.ComponentType"/>
        /// </summary>
        public override Type ComponentType
        {
            get { return null; }
        }

        /// <summary>
        /// Implementation for <see cref="PropertyDescriptor.CanResetValue(object)"/>
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override bool CanResetValue(object component)
        {
            return false;
        }

        /// <summary>
        /// Implementation for <see cref="PropertyDescriptor.ResetValue(object)"/>
        /// </summary>
        /// <param name="component"></param>
        public override void ResetValue(object component)
        {
        }

        /// <summary>
        /// Implementation for <see cref="PropertyDescriptor.ShouldSerializeValue(object)"/>
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
