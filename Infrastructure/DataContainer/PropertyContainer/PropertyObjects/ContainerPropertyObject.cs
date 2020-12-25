using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using KEI.Infrastructure.Types;

namespace KEI.Infrastructure
{
    /// <summary>
    /// PropertyObject implementation for <see cref="IDataContainer"/>
    /// </summary>
    internal class ContainerPropertyObject : PropertyObject
    {
        private IDataContainer _container;

        /// <summary>
        /// Contructor to initialize with <see cref="object"/>
        /// Object is converted to <see cref="IDataContainer"/> using <see cref="PropertyContainerBuilder.CreateObject(string, object)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ContainerPropertyObject(string name, object value)
        {
            Name = name;

            if (value is IDataContainer dc)
            {
                _container = dc;

                if (dc.UnderlyingType is TypeInfo t)
                {
                    Value = dc.Morph();
                    ObjectType = t;
                }
            }
            else
            {
                Value = value;
                ObjectType = value.GetType();
            }

            /// If value implements <see cref="INotifyPropertyChanged"/> subscribe to <see cref="INotifyPropertyChanged.PropertyChanged"/>
            /// and invoke PropertyChanged event on ourselves whenever a property of <see cref="Value"/> changes
            if (Value is INotifyPropertyChanged inpc)
            {
                inpc.PropertyChanged += Inpc_PropertyChanged;
            }
        }

        /// <summary>
        /// Value held by object
        /// </summary>
        private object _value;
        public object Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        /// <summary>
        /// Type of object held
        /// </summary>
        public Type ObjectType { get; set; }

        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => DataObjectType.Container;

        /// <summary>
        /// Implementation for <see cref="DataObject.CanConvertFromString(string)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool CanConvertFromString(string value) => false;

        /// <summary>
        /// Implementation for <see cref="DataObject.GetValue"/>
        /// </summary>
        /// <returns></returns>
        public override object GetValue()
        {
            return _container ?? Value;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.SetValue(object)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool SetValue(object value)
        {
            if(Value.GetType() != value.GetType())
            {
                return false;
            }

            Value = value;

            return true;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.GetDataType"/>
        /// </summary>
        /// <returns></returns>
        public override Type GetDataType()
        {
            return ObjectType ?? typeof(PropertyContainer);
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.InitializeObject"/>
        /// </summary>
        protected override void InitializeObject()
        {
            _container = new PropertyContainer();
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.OnXmlReadingCompleted"/>
        /// </summary>
        protected override void OnXmlReadingCompleted()
        {
            _container.Name = Name;
            if (ObjectType is not null)
            {
                _container.UnderlyingType = ObjectType;

                Value = _container.Morph();

                /// If value implements <see cref="INotifyPropertyChanged"/> subscribe to <see cref="INotifyPropertyChanged.PropertyChanged"/>
                /// and invoke PropertyChanged event on ourselves whenever a property of <see cref="Value"/> changes
                if (Value is INotifyPropertyChanged inpc)
                {
                    inpc.PropertyChanged += Inpc_PropertyChanged;
                }

                // free memory, we don't need it anymore
                _container = null;
            }
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.GetStartElementName"/>
        /// </summary>
        /// <returns></returns>
        protected override string GetStartElementName() => ContainerDataObject.DC_START_ELEMENT_NAME;

        /// <summary>
        /// Implementation for <see cref="DataObject.CanWriteValueAsXmlAttribute"/>
        /// </summary>
        /// <returns></returns>
        protected override bool CanWriteValueAsXmlAttribute() => false;

        /// <summary>
        /// Implementation for <see cref="DataObject.ReadXmlElement(string, XmlReader)"/>
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            // call base implementation
            if (base.ReadXmlElement(elementName, reader))
            {
                return true;
            }

            /// Read underlying type for <see cref="ContainerDataObject"/> created from CLR objects
            if (elementName == nameof(TypeInfo))
            {
                ObjectType = reader.ReadObjectXml<TypeInfo>();

                return true;
            }

            /// Read <see cref="DataObject"/> implementation
            else
            {
                if (DataObjectFactory.GetPropertyObject(reader.GetAttribute(TYPE_ID_ATTRIBUTE)) is DataObject obj)
                {
                    using var newReader = XmlReader.Create(new StringReader(reader.ReadOuterXml()));

                    newReader.Read();

                    obj.ReadXml(newReader);

                    if (obj is not NotSupportedDataObject)
                    {
                        _container.Add(obj);
                    }

                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Implemementation for <see cref="DataObject.WriteXmlContent(XmlWriter)"/>
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlContent(XmlWriter writer)
        {
            // Write type if this based on an object
            if (ObjectType is not null)
            {
                writer.WriteObjectXml(new TypeInfo(ObjectType));
            }

            foreach (var obj in _container ?? PropertyContainerBuilder.CreateObject(Name, Value))
            {
                obj.WriteXml(writer);
            }
        }

        /// <summary>
        /// Inform that our value changed in any of our objects property changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Inpc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Value));
        }

    }
}
