using System;
using System.ComponentModel;

namespace KEI.Infrastructure
{
    public class DataObjectPropertyDescriptor : PropertyDescriptor
    {
        public DataObject DataObject { get; set; }

        internal DataObjectPropertyDescriptor(DataObject obj, Attribute[] attributes)
            : base(obj.Name, attributes)
        {
            DataObject = obj;
        }

        public override Type PropertyType
        {
            get { return DataObject.GetDataType(); }
        }

        public override void SetValue(object component, object value)
        {
            DataObject.SetValue(value);
        }

        public override object GetValue(object component)
        {
            return DataObject.GetValue();
        }

        public override bool IsReadOnly
        {
            get
            {
                return DataObject is PropertyObject po && po.BrowseOption == BrowseOptions.NonEditable;
            }
        }

        public override Type ComponentType
        {
            get { return null; }
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
