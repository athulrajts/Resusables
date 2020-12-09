using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.ObjectModel;
using KEI.Infrastructure.Types;
using System.Collections.Generic;

namespace KEI.Infrastructure
{
    /// <summary>
    /// <see cref="DataObject"/> implementation to store <see cref="IList"/> of non primitive types
    /// </summary>
    internal class ContainerCollectionDataObject : DataObject
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ContainerCollectionDataObject(string name, IList value)
        {
            Name = name;

            int count = 0;
            foreach (var item in value)
            {
                Value.Add(DataContainerBuilder.CreateObject($"{name}[{count++}]", item));
            }

            CollectionType = value.GetType();
        }

        public ContainerCollectionDataObject(string name, ObservableCollection<IDataContainer> value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Holds type of <see cref="IList"/>
        /// </summary>
        public Type CollectionType { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public ObservableCollection<IDataContainer> Value { get; set; } = new ObservableCollection<IDataContainer>();

        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => "dcl";

        /// <summary>
        /// Implementation for <see cref="DataObject.GetDataType"/>
        /// </summary>
        /// <returns></returns>
        public override Type GetDataType()
        {
            return CollectionType;
        }

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
            if (Value is not ObservableCollection<IDataContainer>)
            {
                return false;
            }

            Value = value as ObservableCollection<IDataContainer>;

            return true;
        }

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
        /// Implementation for <see cref="DataObject.WriteXmlContent(XmlWriter)"/>
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlContent(XmlWriter writer)
        {
            // Write base impl
            base.WriteXmlContent(writer);

            // Write collection type
            if (CollectionType is not null)
            {
                writer.WriteObjectXml(new TypeInfo(CollectionType));
            }

            // Write values
            foreach (IDataContainer dc in Value)
            {
                new ContainerDataObject(dc.Name, dc).WriteXml(writer);
            }
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.ReadXmlElement(string, XmlReader)"/>
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            // call base
            if (base.ReadXmlElement(elementName, reader))
            {
                return true;
            }

            // Read DataObject Implementation
            if (elementName == ContainerDataObject.DC_START_ELEMENT_NAME)
            {
                var obj = DataObjectFactory.GetDataObject("dc");

                if (obj is ContainerDataObject cdo)
                {
                    using var newReader = XmlReader.Create(new StringReader(reader.ReadOuterXml()));

                    newReader.Read();

                    cdo.ReadXml(newReader);

                    Value.Add(cdo.Value);

                    return true;
                }
            }

            // Read type info
            else if (elementName == nameof(TypeInfo))
            {
                CollectionType = reader.ReadObjectXml<TypeInfo>();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.InitializeObject"/>
        /// </summary>
        protected override void InitializeObject()
        {
            Value = new ObservableCollection<IDataContainer>();
        }
    }
}
