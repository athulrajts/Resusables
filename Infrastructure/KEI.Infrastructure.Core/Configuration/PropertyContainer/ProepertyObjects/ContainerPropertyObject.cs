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
        public IDataContainer Value { get; set; }

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
        /// Constructor to initialize with <see cref="IList"/>
        /// List is converter to <see cref="IDataContainer"/> using <see cref="PropertyContainerBuilder.CreateList(string, IList)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ContainerPropertyObject(string name, IList value)
        {
            Name = name;
            Value = PropertyContainerBuilder.CreateList(name, value);
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => "dc";

        /// <summary>
        /// Implementation for <see cref="PropertyObject.Editor"/>
        /// </summary>
        public override EditorType Editor => EditorType.Object;

        /// <summary>
        /// Implementation for <see cref="DataObject.ReadXmlElement(string, XmlReader)"/>
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            // Read inner property
            if(elementName == START_ELEMENT)
            {
                var obj = DataObjectFactory.GetPropertyObject(reader.GetAttribute(TYPE_ID_ATTRIBUTE));

                if (obj is not null)
                {
                    // get PropertyObject identified by type attribute 
                    using var newReader = XmlReader.Create(new StringReader(reader.ReadOuterXml()));

                    newReader.Read();

                    obj.ReadXml(newReader);

                    Value.Add(obj);
                }

                return true;
            }

            // Reader inner property container
            else if(elementName == "DataContainer")
            {
                // get ContainerPropertyObject
                var obj = (ContainerPropertyObject)DataObjectFactory.GetPropertyObject("dc");

                if (obj is not null)
                {
                    obj.Value = new PropertyContainer();

                    using var newReader = XmlReader.Create(new StringReader(reader.ReadOuterXml()));

                    newReader.Read();

                    obj.ReadXml(newReader);

                    Value.Add(obj);
                }

                return true;
            }

            // Read TypInfo if exists
            else if(elementName == nameof(TypeInfo))
            {
                Value.UnderlyingType = reader.ReadObjectXML<TypeInfo>();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Implemementation for <see cref="DataObject.WriteXmlInternal(XmlWriter)"/>
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlInternal(XmlWriter writer)
        {
            // Write name
            if(string.IsNullOrEmpty(Name) == false)
            {
                writer.WriteAttributeString(KEY_ATTRIBUTE, Name);
            }

            // Write browse Option
            if (BrowseOption != BrowseOptions.Browsable)
            {
                writer.WriteAttributeString(BROWSE_ATTRIBUTE, BrowseOption.ToString());
            }

            // Write description
            if (string.IsNullOrEmpty(Description) == false)
            {
                writer.WriteElementString(nameof(Description), Description);
            }

            // Write type if this based on an object
            if (Value.UnderlyingType is not null)
            {
                writer.WriteObjectXML(Value.UnderlyingType);
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
