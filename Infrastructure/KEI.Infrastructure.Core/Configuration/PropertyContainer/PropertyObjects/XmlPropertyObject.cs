using System;
using System.Xml;
using KEI.Infrastructure.Types;

namespace KEI.Infrastructure
{
    internal class XmlPropertyObject : PropertyObject
    {
        const string XML_TAG = "XML";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public XmlPropertyObject(string name, object value)
        {
            Name = name;
            Value = value;
            ObjectType = value.GetType();
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => DataObjectType.Xml;

        /// <summary>
        /// Holds the value
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
        /// Implementation for <see cref="DataObject.GetDataType"/>
        /// </summary>
        /// <returns></returns>
        public override Type GetDataType()
        {
            return ObjectType;
        }

        /// <summary>
        /// Implementation for <see cref="GetValue"/>
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
            if(value.GetType() != ObjectType)
            {
                return false;
            }

            Value = value;

            return true;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.CanWriteValueAsXmlAttribute"/>
        /// </summary>
        /// <returns></returns>
        protected override bool CanWriteValueAsXmlAttribute()
        {
            return false;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.WriteXmlContent(XmlWriter)"/>
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlContent(XmlWriter writer)
        {
            base.WriteXmlContent(writer);

            // write type so when we can deserialize
            writer.WriteObjectXml(new TypeInfo(ObjectType));

            writer.WriteStartElement(XML_TAG);
            string xmlString = XmlHelper.SerializeToString(Value, true);
            writer.WriteCData(xmlString);
            writer.WriteEndElement();
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

            // read object type
            if(elementName == nameof(TypeInfo))
            {
                ObjectType = reader.ReadObjectXml<TypeInfo>();

                return true;
            }

            // read xml string
            else if(elementName == XML_TAG)
            {
                reader.Read();

                if(reader.NodeType == XmlNodeType.CDATA)
                {
                    StringValue = reader.Value;
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
            Value = XmlHelper.DeserializeFromString(ObjectType, StringValue);
        }
    }
}
