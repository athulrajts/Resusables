using KEI.Infrastructure.Types;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;

namespace KEI.Infrastructure
{
    /// <summary>
    /// PropertyObject Implementation for storing <see cref="IList"/> of not primitive types
    /// </summary>
    internal class ContainerCollectionPropertyObject : PropertyObject
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ContainerCollectionPropertyObject(string name, IList value)
        {
            Name = name;

            int count = 0;
            foreach (var item in value)
            {
                Value.Add((PropertyContainer)PropertyContainerBuilder.CreateObject($"{name}[{count++}]", item));
            }

            CollectionType = value.GetType();
        }

        /// <summary>
        /// Type of <see cref="IList"/>
        /// </summary>
        public Type CollectionType { get; set; }

        /// <summary>
        /// Implementation for <see cref="PropertyObject.Editor"/>
        /// </summary>
        public override EditorType Editor => EditorType.Object;

        /// <summary>
        /// Value
        /// </summary>
        public ObservableCollection<IDataContainer> Value { get; set; } = new ObservableCollection<IDataContainer>();

        /// <summary>
        /// Imlementation for <see cref="DataObject.Type"/>
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
        /// Implementation for <see cref="DataObject.GetValue()"/>
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
            if(Value is not ObservableCollection<IDataContainer>)
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

            // Write category
            if (string.IsNullOrEmpty(Category) == false)
            {
                writer.WriteElementString(nameof(Category), Category);
            }

            // Write display Name
            if (string.IsNullOrEmpty(DisplayName) == false)
            {
                writer.WriteElementString(nameof(DisplayName), DisplayName);
            }

            // Write collection type
            if (CollectionType is not null)
            {
                writer.WriteObjectXml(new TypeInfo(CollectionType));
            }

            // Write values
            foreach (var dc in Value)
            {
                new ContainerPropertyObject(dc.Name, dc).WriteXml(writer);
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
            if(base.ReadXmlElement(elementName, reader))
            {
                return true;
            }

            if(elementName == "DataContainer")
            {
                using var newReader = XmlReader.Create(new StringReader(reader.ReadOuterXml()));
                newReader.Read();
                
                var dco = DataObjectFactory.GetPropertyObject("dc");
                dco.ReadXml(newReader);

                Value.Add(dco.GetValue() as PropertyContainer);

                return true;
            }
            else if(elementName == nameof(TypeInfo))
            {
                CollectionType = reader.ReadObjectXml<TypeInfo>();
            }

            return false;
        }

        /// <summary>
        /// Implementatino for <see cref="DataObject.InitializeObject"/>
        /// </summary>
        protected override void InitializeObject()
        {
            Value = new ObservableCollection<IDataContainer>();
        }
    }
}
