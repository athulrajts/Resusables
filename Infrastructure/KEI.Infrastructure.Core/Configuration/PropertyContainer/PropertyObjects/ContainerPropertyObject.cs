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
        private IDataContainer _value;
        public IDataContainer Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        /// <summary>
        /// Constructor to initialize with <see cref="IDataContainer"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ContainerPropertyObject(string name, IDataContainer value)
        {
            Value = value;
            Value.Name = name;
            Name = name;
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

            Value = PropertyContainerBuilder.CreateObject(name, value);
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
        /// Implementation for <see cref="DataObject.InitializeObject"/>
        /// </summary>
        protected override void InitializeObject()
        {
            Value = new PropertyContainer();
        }

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
                Value.UnderlyingType = reader.ReadObjectXml<TypeInfo>();

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
                        Value.Add(obj); 
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
            if (Value.UnderlyingType is not null)
            {
                writer.WriteObjectXml(Value.UnderlyingType);
            }

            // Write inner properties
            foreach (var obj in Value)
            {
                obj.WriteXml(writer);
            }
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.OnXmlReadingCompleted"/>
        /// </summary>
        protected override void OnXmlReadingCompleted()
        {
            Value.Name = Name;
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
            return Value;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.SetValue(object)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool SetValue(object value)
        {
            if(value is not IDataContainer)
            {
                return false;
            }

            Value = (IDataContainer)value;

            return true;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.GetDataType"/>
        /// </summary>
        /// <returns></returns>
        public override Type GetDataType()
        {
            return typeof(PropertyContainer);
        }

    }
}
