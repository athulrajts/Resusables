using KEI.Infrastructure.Types;
using System;
using System.Collections;
using System.IO;
using System.Xml;

namespace KEI.Infrastructure
{
    /// <summary>
    /// DataObject implementation for <see cref="IDataContainer"/>
    /// </summary>
    internal class ContainerDataObject : DataObject, IWriteToBinaryStream
    {
        public const string DC_START_ELEMENT_NAME = "DataContainer";

        public IDataContainer Value { get; set; }

        /// <summary>
        /// Construct to create object from <see cref="IDataContainer"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ContainerDataObject(string name, IDataContainer value)
        {
            Value = value;
            Value.Name = name;
            Name = name;
        }

        /// <summary>
        /// Constructor to create object of any objects.
        /// converts given objects to IDataContainer using <see cref="DataContainerBuilder.CreateObject(string, object)"/>
        /// </summary>
        /// <param name="name">name/key</param>
        /// <param name="value">value</param>
        public ContainerDataObject(string name, object value)
        {
            Name = name;

            Value = DataContainerBuilder.CreateObject(name, value);
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => "dc";

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
            if (value is not IDataContainer)
            {
                return false;
            }

            Value = (IDataContainer)value;

            return true;
        }

        /// <summary>
        /// Implementation for <see cref="IWriteToBinaryStream.WriteBytes(BinaryWriter)"/>
        /// </summary>
        /// <param name="writer"></param>
        public void WriteBytes(BinaryWriter writer)
        {
            foreach (var item in Value)
            {
                if (item is IWriteToBinaryStream wbs)
                {
                    wbs.WriteBytes(writer);
                }
            }

        }

        /// <summary>
        /// Implementation of <see cref="DataObject.GetDataType"/>
        /// </summary>
        /// <returns></returns>
        public override Type GetDataType()
        {
            return typeof(DataContainer);
        }

        #region XmlSerialization members

        protected override void InitializeObject()
        {
            Value = new DataContainer();
        }

        protected override string GetStartElementName()
        {
            return DC_START_ELEMENT_NAME;
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
            if(base.ReadXmlElement(elementName, reader))
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
                if (DataObjectFactory.GetDataObject(reader.GetAttribute(TYPE_ID_ATTRIBUTE)) is DataObject obj)
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
        /// Implementation for <see cref="DataObject.OnXmlReadingCompleted"/>
        /// </summary>
        protected override void OnXmlReadingCompleted()
        {
            Value.Name = Name;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.WriteXmlContent(XmlWriter)"/>
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlContent(XmlWriter writer)
        {
            if (Value.UnderlyingType is not null)
            {
                writer.WriteObjectXml(Value.UnderlyingType);
            }

            foreach (var obj in Value)
            {
                obj.WriteXml(writer);
            }
        }

        #endregion


    }
}
