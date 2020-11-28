using KEI.Infrastructure.Configuration;
using KEI.Infrastructure.Types;
using System;
using System.Collections;
using System.IO;
using System.Xml;

namespace KEI.Infrastructure
{
    public class ContainerDataObject : DataObject, IWriteToBinaryStream
    {
        public IDataContainer Value { get; set; }

        public ContainerDataObject(string name, IDataContainer value)
        {
            Value = value;
            Value.Name = name;
            Name = name;
        }

        public ContainerDataObject(string name, object value)
        {
            Name = name;

            Value = DataContainerBuilder.CreateObject(name, value);
        }

        public ContainerDataObject(string name, IList value)
        {
            Name = name;
            Value = DataContainerBuilder.CreateList(name, value);
        }

        public override string Type => "dc";

        public override object GetValue()
        {
            return Value;
        }

        public override bool SetValue(object value)
        {
            if (value is not IDataContainer)
            {
                return false;
            }

            Value = (IDataContainer)value;

            return true;
        }

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

        protected override bool ReadElement(string elementName, XmlReader reader)
        {
            if (elementName == START_ELEMENT)
            {
                var obj = DataObjectFactory.GetDataObject(reader.GetAttribute(TYPE_ID_ATTRIBUTE));

                if (obj is not null)
                {
                    using var newReader = XmlReader.Create(new StringReader(reader.ReadOuterXml()));

                    newReader.Read();

                    obj.ReadXml(newReader);

                    Value.Add(obj);
                }

                return true;
            }
            else if (elementName == "DataContainer")
            {
                var obj = (ContainerDataObject)DataObjectFactory.GetDataObject("dc");

                if (obj is not null)
                {
                    obj.Value = new DataContainer();

                    using var newReader = XmlReader.Create(new StringReader(reader.ReadOuterXml()));

                    newReader.Read();

                    obj.ReadXml(newReader);

                    Value.Add(obj);
                }

                return true;
            }
            else if (elementName == nameof(TypeInfo))
            {
                Value.UnderlyingType = reader.ReadObjectXML<TypeInfo>();

                return true;
            }

            return false;
        }


        protected override void WriteXmlInternal(XmlWriter writer)
        {
            if (string.IsNullOrEmpty(Name) == false)
            {
                writer.WriteAttributeString(KEY_ATTRIBUTE, Name);
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

        public override Type GetDataType()
        {
            return typeof(DataContainer);
        }

        protected override void OnReadingCompleted()
        {
            Value.Name = Name;
        }

    }
}
