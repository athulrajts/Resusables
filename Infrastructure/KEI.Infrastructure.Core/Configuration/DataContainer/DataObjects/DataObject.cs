using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;
using Prism.Mvvm;
using System.Collections;

namespace KEI.Infrastructure
{
    internal interface IWriteToBinaryStream
    {
        public void WriteBytes(BinaryWriter writer);
    }

    /// <summary>
    /// Abstraction for holding different type of data in <see cref="IDataContainer"/>
    /// Handles Xml Serialization and Deserialization
    /// Handles validation when updating values.
    /// </summary>
    public abstract class DataObject : BindableBase, IXmlSerializable
    {
        // constants for xml serialization
        public const string KEY_ATTRIBUTE = "key";
        public const string VALUE_ATTRIBUTE = "value";
        public const string TYPE_ID_ATTRIBUTE = "type";
        public const string START_ELEMENT = "Data";

        public DataObject()
        {

        }

        /// <summary>
        /// Unique name/key
        /// </summary>
        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        /// <summary>
        /// Value as string if possible
        /// </summary>
        protected string stringValue;
        public virtual string StringValue
        {
            get { return stringValue; }
            set { SetProperty(ref stringValue, value, () => OnStringValueChanged(value)); }
        }

        /// <summary>
        /// Type of DataObject
        /// </summary>
        public abstract string Type { get; }

        /// <summary>
        /// Checks whether we can convert the given string this objects value
        /// </summary>
        /// <param name="value">value to check</param>
        /// <returns></returns>
        public virtual bool CanConvertFromString(string value) { return true; }

        /// <summary>
        /// Gets object that can be used to update our value from given string
        /// </summary>
        /// <param name="value">value to update</param>
        /// <returns></returns>
        public virtual object ConvertFromString(string value) { return null; }

        /// <summary>
        /// Gets value held by this object
        /// </summary>
        /// <returns></returns>
        public abstract object GetValue();

        /// <summary>
        /// Sets value held by this object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool SetValue(object value);

        /// <summary>
        /// Sets value held by this object from given string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetValueFromString(string value)
        {
            return CanConvertFromString(value) && SetValue(ConvertFromString(value));
        }

        /// <summary>
        /// Override ToString() to return string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString() => StringValue;

        /// <summary>
        /// Gets the type of value this objects holds.
        /// </summary>
        /// <returns></returns>
        public abstract Type GetDataType();

        #region XmlSerialization Members

        public XmlSchema GetSchema() => null;

        /// <summary>
        /// Update values from XML
        /// </summary>
        /// <param name="reader">XmlReader to read</param>
        public virtual void ReadXml(XmlReader reader)
        {
            InitializeObject();

            // Read all the attributes.
            ReadXmlAttributes(reader);

            reader.Read();

            // Loop till we finisehd reading
            while (reader.EOF == false)
            {
                /// Encountered element of type <see cref="XmlNodeType.Element"/>
                /// Implementations should override <see cref="ReadXmlElement(string, XmlReader)"/>
                /// to read the element.
                if (reader.NodeType == XmlNodeType.Element)
                {
                    /// If implementation doesn't provide way to read the element, skip.
                    if (ReadXmlElement(reader.Name, reader) == false)
                    {
                        reader.Read();
                    }
                }

                // nothing of intereset, skip.
                else
                {
                    reader.Read();
                }
            }

            // Finish up reading after all required values have been read.
            OnXmlReadingCompleted();

        }

        /// <summary>
        /// Read all the attributes from XML
        /// reads <see cref="KEY_ATTRIBUTE"/> and <see cref="VALUE_ATTRIBUTE"/> if exists
        /// as default implementation
        /// </summary>
        /// <param name="reader"></param>
        protected virtual void ReadXmlAttributes(XmlReader reader)
        {
            Name = reader.GetAttribute(KEY_ATTRIBUTE);

            if (reader.GetAttribute(VALUE_ATTRIBUTE) is string attr)
            {
                StringValue = attr;
            }
        }

        /// <summary>
        /// This function wil lbe called whenever an <see cref="XmlNodeType.Element"/> is
        /// encountered while read
        /// </summary>
        /// <param name="elementName">Value of <see cref="XmlReader.Name"/></param>
        /// <param name="reader">XmlReader to complete reading element</param>
        /// <returns>returns true when reading was finished succesfully</returns>
        protected virtual bool ReadXmlElement(string elementName, XmlReader reader)
        {
            return false;
        }

        /// <summary>
        /// Called when reading from Xml is done completely
        /// can be used by implementations to use initialize objects after all
        /// required values are read.
        /// </summary>
        protected virtual void OnXmlReadingCompleted() { }

        /// <summary>
        /// Name of starting tag when writing as Xml
        /// </summary>
        /// <returns></returns>
        protected virtual string GetStartElementName() { return START_ELEMENT; }

        /// <summary>
        /// Writes value to an XmlWriter
        /// </summary>
        /// <param name="writer"></param>
        public virtual void WriteXml(XmlWriter writer)
        {
            // write start tag
            writer.WriteStartElement(GetStartElementName());

            // write attributes
            WriteXmlAttributes(writer);

            // write content
            WriteXmlContent(writer);

            // write end tag
            writer.WriteEndElement();
        }

        protected virtual void WriteXmlAttributes(XmlWriter writer)
        {
            writer.WriteAttributeString(TYPE_ID_ATTRIBUTE, Type);
            writer.WriteAttributeString(KEY_ATTRIBUTE, Name);

            if (string.IsNullOrEmpty(StringValue) == false)
            {
                writer.WriteAttributeString(VALUE_ATTRIBUTE, StringValue);
            }
        }

        /// <summary>
        /// Writes the state of the object to an XmlWriter
        /// Writes <see cref="Type"/>, <see cref="Name"/>  and <see cref="StringValue"/> as attributes by default
        /// </summary>
        /// <param name="writer"></param>
        protected virtual void WriteXmlContent(XmlWriter writer) { }

        #endregion

        /// <summary>
        /// Called when <see cref="StringValue"/> is set
        /// Used to update the value held by this object when <see cref="StringValue"/>
        /// is updated from editor.
        /// </summary>
        /// <param name="value"></param>
        protected virtual void OnStringValueChanged(string value) { }

        protected virtual void InitializeObject() { }
    }

    /// <summary>
    /// Generic implementation for DataObject for primitive types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DataObject<T> : DataObject
    {
        /// <summary>
        /// Value held by this object
        /// </summary>
        protected T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                if (EqualityComparer<T>.Default.Equals(_value, value) == true)
                {
                    return;
                }

                _value = value;

                // update string value whenever our value changes
                stringValue = ConvertToString(_value);

                RaisePropertyChanged(nameof(Value));
                RaisePropertyChanged(nameof(StringValue));
            }
        }

        /// <summary>
        /// Convert value held to string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual string ConvertToString(T value)
        {
            return value?.ToString();
        }


        /// <summary>
        /// Gets value held by object
        /// </summary>
        /// <returns></returns>
        public override object GetValue() => Value;

        /// <summary>
        /// Sets value held by object
        /// </summary>
        /// <param name="value">returns true if value was updated</param>
        /// <returns></returns>
        public override bool SetValue(object value)
        {
            if (value is not T)
            {
                return false;
            }

            Value = (T)value;

            return true;
        }

        /// <summary>
        /// Return type of value held by object
        /// </summary>
        /// <returns></returns>
        public override Type GetDataType() => typeof(T);

        /// <summary>
        /// Default contructor
        /// </summary>
        public DataObject()
        {

        }
    }
}
