using System;
using System.IO;
using System.Xml;
using System.Collections;
using KEI.Infrastructure.Types;

namespace KEI.Infrastructure
{
    /// <summary>
    /// PropertyObject implementation for <see cref="IDataContainer"/>
    /// </summary>
    internal class ContainerPropertyObject : PropertyObject
    {
        private IDataContainer _container;

        private object _value;
        public object Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        public Type ObjectType { get; set; }

        /// <summary>
        /// Constructor to initialize with <see cref="IDataContainer"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ContainerPropertyObject(string name, IDataContainer value)
        {
            Name = name;
            
            if (value.UnderlyingType is TypeInfo t)
            {
                Value = value.Morph();
                ObjectType = t;
            }
            
            _container = value;
        }

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
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => "dc";

        /// <summary>
        /// Implementation for <see cref="DataObject.GetStartElementName"/>
        /// </summary>
        /// <returns></returns>
        protected override string GetStartElementName()
        {
            return ContainerDataObject.DC_START_ELEMENT_NAME;
        }

        protected override bool CanWriteValueAsXmlAttribute() { return false; }

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
        /// Implementation for <see cref="DataObject.OnXmlReadingCompleted"/>
        /// </summary>
        protected override void OnXmlReadingCompleted()
        {
            _container.Name = Name;
            if(ObjectType is not null)
            {
                _container.UnderlyingType = ObjectType;

                Value = _container.Morph();
                // free memory, we don't need it anymore
                _container = null;
            }
        }

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


        protected override void InitializeObject()
        {
            _container = new PropertyContainer();
        }
    }
}
