using Prism.Mvvm;
using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace KEI.Infrastructure
{
    internal interface IWriteToBinaryStream
    {
        public void WriteBytes(BinaryWriter writer);
    }

    public abstract class DataObject : BindableBase, IXmlSerializable
    {
        public const string KEY_ATTRIBUTE = "key";
        public const string VALUE_ATTRIBUTE = "value";
        public const string TYPE_ID_ATTRIBUTE = "type";
        public const string START_ELEMENT = "Data";

        public DataObject()
        {

        }

        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        protected string stringValue;
        public virtual string StringValue
        {
            get { return stringValue; }
            set { SetProperty(ref stringValue, value, () => OnStringValueChanged(value)); }
        }

        public abstract string Type { get; }

        public virtual bool ValidateForType(string value) { return true; }

        protected virtual void OnStringValueChanged(string value) { }

        public abstract object GetValue();
        public abstract bool SetValue(object value);

        public override string ToString() => StringValue;

        public XmlSchema GetSchema() => null;

        public virtual void ReadXml(XmlReader reader)
        {
            ReadAttrubutes(reader);

            reader.Read();

            while (reader.EOF == false)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (ReadElement(reader.Name, reader) == false)
                    {
                        reader.Read();
                    }
                }
                else
                {
                    reader.Read();
                }
            }

            OnReadingCompleted();

        }

        protected virtual void ReadAttrubutes(XmlReader reader)
        {
            Name = reader.GetAttribute(KEY_ATTRIBUTE);

            if (reader.GetAttribute(VALUE_ATTRIBUTE) is string attr)
            {
                StringValue = attr;
            }
        }

        protected virtual bool ReadElement(string elementName, XmlReader reader)
        {
            return false;
        }

        protected virtual void OnReadingCompleted() { }

        public virtual void WriteXml(XmlWriter writer)
        {
            string elementName = (this is ContainerDataObject || this is ContainerPropertyObject) ? "DataContainer" : START_ELEMENT;

            writer.WriteStartElement(elementName);

            WriteXmlInternal(writer);

            writer.WriteEndElement();
        }

        protected virtual void WriteXmlInternal(XmlWriter writer)
        {
            writer.WriteAttributeString(TYPE_ID_ATTRIBUTE, Type);
            writer.WriteAttributeString(KEY_ATTRIBUTE, Name);
            writer.WriteAttributeString(VALUE_ATTRIBUTE, StringValue);
        }

        public abstract Type GetDataType();
    }

    public abstract class DataObject<T> : DataObject
    {

        protected T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                SetProperty(ref _value, value);

                stringValue = _value.ToString();

                RaisePropertyChanged(nameof(StringValue));
            }
        }

        public override object GetValue() => Value;

        public override bool SetValue(object value)
        {
            if (value.GetType() != typeof(T))
            {
                return false;
            }

            Value = (T)value;

            return true;
        }

        protected override void OnStringValueChanged(string value)
        {
            _value = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertTo(value, typeof(T));

            RaisePropertyChanged(nameof(Value));
        }

        public override Type GetDataType()
        {
            return typeof(T);
        }

        public DataObject()
        {

        }
    }
}
