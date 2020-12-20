using System;
using System.Xml;
using System.Text.Json;
using KEI.Infrastructure.Types;

namespace KEI.Infrastructure
{
    internal class JsonPropertyObject : PropertyObject
    {
        const string JSON_TAG = "JSON";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public JsonPropertyObject(string name, object value)
        {
            Name = name;
            Value = value;
            ObjectType = value.GetType();
        }

        /// <summary>
        /// Holds the actual value
        /// </summary>
        private object _value;
        public object Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        /// <summary>
        /// Type of held value
        /// </summary>
        public Type ObjectType { get; set; }

        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => DataObjectType.Json;

        /// <summary>
        /// Implementation for <see cref="DataObject.GetDataType"/>
        /// </summary>
        /// <returns></returns>
        public override Type GetDataType()
        {
            return ObjectType;
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
            if (value.GetType() != ObjectType)
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
        protected override bool CanWriteValueAsXmlAttribute() => false;

        /// <summary>
        /// Implementation for <see cref="DataObject.WriteXmlContent(XmlWriter)"/>
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlContent(XmlWriter writer)
        {
            base.WriteXmlContent(writer);

            // write type so when we can deserialize
            writer.WriteObjectXml(new TypeInfo(ObjectType));

            writer.WriteStartElement(JSON_TAG);
            string jsonString = JsonSerializer.Serialize(Value, ObjectType, new JsonSerializerOptions { WriteIndented = true });
            writer.WriteCData(jsonString);
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
            if (base.ReadXmlElement(elementName, reader))
            {
                return true;
            }

            // read object type
            if (elementName == nameof(TypeInfo))
            {
                ObjectType = reader.ReadObjectXml<TypeInfo>();

                return true;
            }

            // read xml string
            else if (elementName == JSON_TAG)
            {
                reader.Read();

                if (reader.NodeType == XmlNodeType.CDATA)
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
            Value = JsonSerializer.Deserialize(StringValue, ObjectType, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
