using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.ObjectModel;
using KEI.Infrastructure.Types;

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
                Value.Add((DataContainer)DataContainerBuilder.CreateObject($"{name}[{count++}]", item));
            }

            CollectionType = value.GetType();
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
        /// Implementation for <see cref="DataObject.WriteXmlInternal(XmlWriter)"/>
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlInternal(XmlWriter writer)
        {
            // Write name
            if (string.IsNullOrEmpty(Name) == false)
            {
                writer.WriteAttributeString(KEY_ATTRIBUTE, Name);
            }

            // Write collection type
            if (CollectionType is not null)
            {
                writer.WriteObjectXml(new TypeInfo(CollectionType));
            }

            // Write values
            foreach (var dc in Value)
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
            if (base.ReadXmlElement(elementName, reader))
            {
                return true;
            }

            if (elementName == "DataContainer")
            {
                using var newReader = XmlReader.Create(new StringReader(reader.ReadOuterXml()));
                newReader.Read();

                var dco = DataObjectFactory.GetDataObject("dc");
                dco.ReadXml(newReader);

                Value.Add(dco.GetValue() as DataContainer);

                return true;
            }
            else if (elementName == nameof(TypeInfo))
            {
                CollectionType = reader.ReadObjectXml<TypeInfo>();
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
