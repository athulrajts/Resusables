using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using KEI.Infrastructure.Helpers;

namespace KEI.Infrastructure.Configuration
{
    /// <summary>
    /// Abstraction for holding different type of data
    /// Base class for <see cref="PropertyObject"/>
    /// </summary>
    public class DataObject : BindableBase
    { 
        /// <summary>
        /// Name of the Data
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get Only property which gets type object from <see cref="TypeString"/>
        /// </summary>
        public Type Type => _value == null ? null : _value.GetType();

        /// <summary>
        /// Represents the object as a string
        /// In case of Primitive Types converted to string via <see cref="object.ToString()"/> Method
        /// Otherwise it's Serialized into JSON format. <seealso cref="IsPrimitiveType()"/>
        /// </summary>
        protected string valueString = string.Empty;
        public string ValueString
        {
            get => valueString;
            set
            {
                if (valueString == value || value == null)
                    return;

                valueString = value;

                if (Type?.CanConvert(value) == true)
                {
                    _value = Type.ConvertFrom(value);
                }
                else if (_value is Selector s)
                {
                    s.SelectedItem = value;
                }

                RaisePropertyChanged(nameof(ValueString));
                RaisePropertyChanged(Name);
            }
        }

        /// <summary>
        /// Get only property which converts the string value <see cref="ValueString"/>
        /// to an Object of it's type <see cref="Type"/>
        /// </summary>
        protected object _value;
        public virtual object Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value == value || value == null)
                    return;

                _value = value;

                if (Value is Selector s)
                    valueString = s.SelectedItem;
                else
                    valueString = _value.ToString();

                RaisePropertyChanged(nameof(ValueString));
                RaisePropertyChanged(Name);
            }
        }

        public override string ToString() => ValueString;
    }


    /// <summary>
    /// Wrapper to eliminate one node when serializing to XML
    /// </summary>
    public class DataObjectCollection : ObservableCollection<DataObject>
    {
        public DataObjectCollection() : base() { }
        public DataObjectCollection(IEnumerable<DataObject> source) : base(source) { }
    }
}
