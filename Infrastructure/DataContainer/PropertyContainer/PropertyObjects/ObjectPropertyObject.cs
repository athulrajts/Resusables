using System;
using System.Xml;
using System.Text.Json;
using System.ComponentModel;
using KEI.Infrastructure.Types;

namespace KEI.Infrastructure
{
    internal abstract class ObjectPropertyObject : PropertyObject
    {
        /// <summary>
        /// Holds the actual value
        /// </summary>
        protected object _value;
        public object Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        /// <summary>
        /// Type of <see cref="Value"/>
        /// </summary>
        public Type ObjectType { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ObjectPropertyObject(string name, object value)
        {
            Name = name;
            Value = value;
            ObjectType = value.GetType();

            /// If value implements <see cref="INotifyPropertyChanged"/> subscribe to <see cref="INotifyPropertyChanged.PropertyChanged"/>
            /// and invoke PropertyChanged event on ourselves whenever a property of <see cref="Value"/> changes
            if (value is INotifyPropertyChanged inpc)
            {
                inpc.PropertyChanged += Inpc_PropertyChanged;
            }
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~ObjectPropertyObject()
        {
            /// Unhook the property changed listener
            if (Value is INotifyPropertyChanged inpc)
            {
                inpc.PropertyChanged -= Inpc_PropertyChanged;
            }
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.GetDataType"/>
        /// </summary>
        /// <returns></returns>
        public override Type GetDataType() => ObjectType;

        /// <summary>
        /// Implementation for <see cref="DataObject.GetValue"/>
        /// </summary>
        /// <returns></returns>
        public override object GetValue() => Value;

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

            writer.WriteStartElement(GetContentElementTag());
            writer.WriteCData(Serialize());
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
            else if (elementName == GetContentElementTag())
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
            Value = Deserialize();

            /// If value implements <see cref="INotifyPropertyChanged"/> subscribe to <see cref="INotifyPropertyChanged.PropertyChanged"/>
            /// and invoke PropertyChanged event on ourselves whenever a property of <see cref="Value"/> changes
            if (Value is INotifyPropertyChanged inpc)
            {
                inpc.PropertyChanged += Inpc_PropertyChanged;
            }
        }

        /// <summary>
        /// Implementors should return a serialized string value using <see cref="Value"/>
        /// </summary>
        /// <returns></returns>
        protected abstract string Serialize();

        /// <summary>
        /// Implementors should return deserialized object of type <see cref="ObjectType"/> from <see cref="DataObject.StringValue"/>
        /// </summary>
        /// <returns></returns>
        protected abstract object Deserialize();

        /// <summary>
        /// Element name in which the the <see cref="XmlNodeType.CDATA"/> node resides which contains
        /// the value returned by <see cref="Serialize"/>
        /// </summary>
        /// <returns></returns>
        protected abstract string GetContentElementTag();

        /// <summary>
        /// Inform that our value changed in any of our objects property changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Inpc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Value));
        }
    }



    /// <summary>
    /// DataObject implementation for storing POCO's as Json
    /// </summary>
    internal class JsonPropertyObject : ObjectPropertyObject
    {
        const string JSON_TAG = "JSON";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public JsonPropertyObject(string name, object value) : base(name, value) { }

        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => DataObjectType.Json;

        /// <summary>
        /// Implementation for <see cref="ObjectDataObject.Deserialize"/>
        /// </summary>
        /// <returns></returns>
        protected override object Deserialize()
        {
            return JsonSerializer.Deserialize(StringValue, ObjectType, new JsonSerializerOptions { WriteIndented = true });
        }

        /// <summary>
        /// Implementation for <see cref="ObjectDataObject.GetContentElementTag"/>
        /// </summary>
        /// <returns></returns>
        protected override string GetContentElementTag() => JSON_TAG;

        /// <summary>
        /// Implementation for <see cref="ObjectDataObject.Serialize"/>
        /// </summary>
        /// <returns></returns>
        protected override string Serialize()
        {
            return JsonSerializer.Serialize(Value, ObjectType, new JsonSerializerOptions { WriteIndented = true });
        }
    }

    /// <summary>
    /// DataObject implementation for storing POCO's as Xml
    /// </summary>
    internal class XmlPropertyObject : ObjectPropertyObject
    {
        const string XML_TAG = "XML";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public XmlPropertyObject(string name, object value) : base(name, value) { }

        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => DataObjectType.Xml;

        /// <summary>
        /// Implementation for <see cref="ObjectDataObject.Deserialize"/>
        /// </summary>
        /// <returns></returns>
        protected override object Deserialize()
        {
            return XmlHelper.DeserializeFromString(ObjectType, StringValue);
        }

        /// <summary>
        /// Implementation for <see cref="ObjectDataObject.GetContentElementTag"/>
        /// </summary>
        /// <returns></returns>
        protected override string GetContentElementTag() => XML_TAG;

        /// <summary>
        /// Implementation for <see cref="ObjectDataObject.Serialize"/>
        /// </summary>
        /// <returns></returns>
        protected override string Serialize()
        {
            return XmlHelper.SerializeToString(Value, true);
        }
    }
}
