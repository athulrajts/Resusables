using System;
using System.Xml;
using KEI.Infrastructure.Types;
using System.Collections;
using System.IO;

namespace KEI.Infrastructure
{
    public class ContainerPropertyObject : PropertyObject
    {
        public IDataContainer Value { get; set; }

        public ContainerPropertyObject(string name, IDataContainer value)
        {
            Value = value;
            Value.Name = name;
            Name = name;
        }

        public ContainerPropertyObject(string name, object value)
        {
            Name = name;

            Value = PropertyContainerBuilder.CreateObject(name, value);
        }

        public ContainerPropertyObject(string name, IList value)
        {
            Name = name;
            Value = PropertyContainerBuilder.CreateList(name, value);
        }

        public override string Type => "dc";

        public override EditorType Editor => EditorType.Object;

        protected override bool ReadElement(string elementName, XmlReader reader)
        {
            if(elementName == START_ELEMENT)
            {
                var obj = DataObjectFactory.GetPropertyObject(reader.GetAttribute(TYPE_ID_ATTRIBUTE));

                if (obj is not null)
                {
                    using var newReader = XmlReader.Create(new StringReader(reader.ReadOuterXml()));

                    newReader.Read();

                    obj.ReadXml(newReader);

                    Value.Add(obj);
                }

                return true;
            }
            else if(elementName == "DataContainer")
            {
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
            else if(elementName == nameof(TypeInfo))
            {
                Value.UnderlyingType = reader.ReadObjectXML<TypeInfo>();

                return true;
            }

            return false;
        }

        protected override void WriteXmlInternal(XmlWriter writer)
        {
            if(string.IsNullOrEmpty(Name) == false)
            {
                writer.WriteAttributeString(KEY_ATTRIBUTE, Name);
            }

            if (BrowseOption != BrowseOptions.Browsable)
            {
                writer.WriteAttributeString(BROWSE_ATTRIBUTE, BrowseOption.ToString());
            }

            if (string.IsNullOrEmpty(Description) == false)
            {
                writer.WriteElementString(nameof(Description), Description);
            }

            if (Value.UnderlyingType is not null)
            {
                writer.WriteObjectXML(Value.UnderlyingType);
            }

            foreach (var obj in Value)
            {
                obj.WriteXml(writer);
            }
        }

        public override bool ValidateForType(string value) => true;

        public override object GetValue()
        {
            return Value;
        }

        public override bool SetValue(object value)
        {
            if(value is not IDataContainer)
            {
                return false;
            }

            Value = (IDataContainer)value;

            return true;
        }

        public override Type GetDataType()
        {
            return typeof(PropertyContainer);
        }

        protected override void OnReadingCompleted()
        {
            Value.Name = Name;
        }
    }
}
