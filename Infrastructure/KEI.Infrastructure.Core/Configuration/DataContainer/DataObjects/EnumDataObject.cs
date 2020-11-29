﻿using KEI.Infrastructure.Types;
using System;
using System.IO;
using System.Xml;

namespace KEI.Infrastructure
{
    /// <summary>
    /// DataObject implementation for <see cref="enum"/>
    /// </summary>
    internal class EnumDataObject : DataObject<Enum>, IWriteToBinaryStream
    {
        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => "enum";

        /// <summary>
        /// contains Type of enum stored in this object
        /// </summary>
        public Type EnumType { get; set; }

        /// <summary>
        /// Implementation for <see cref="DataObject.ReadXmlElement(string, XmlReader)"/>
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            bool read = false;

            if (elementName == nameof(TypeInfo))
            {
                EnumType = reader.ReadObjectXML<TypeInfo>();

                read = true;
            }

            return read;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.OnXmlReadingCompleted"/>
        /// </summary>
        protected override void OnXmlReadingCompleted()
        {
            Value = (Enum)Enum.Parse(EnumType, StringValue);
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.WriteXmlInternal(XmlWriter)"/>
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlInternal(XmlWriter writer)
        {
            base.WriteXmlInternal(writer);

            writer.WriteObjectXML(new TypeInfo(EnumType));
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.CanConvertFromString(string)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool CanConvertFromString(string value)
        {
            return Enum.TryParse(EnumType, value, out _);
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.ConvertFromString(string)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFromString(string value)
        {
            if(EnumType is null)
            {
                return null;
            }

            return Enum.TryParse(EnumType, value, out object tmp)
                ? tmp
                : null;
        }

        /// <summary>
        /// Implementation for <see cref="IWriteToBinaryStream.WriteBytes(BinaryWriter)"/>
        /// </summary>
        /// <param name="writer"></param>
        public void WriteBytes(BinaryWriter writer)
        {
            writer.Write(Convert.ToInt32(Value));
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.GetDataType"/>
        /// </summary>
        /// <returns></returns>
        public override Type GetDataType()
        {
            return EnumType;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.OnStringValueChanged(string)"/>
        /// </summary>
        /// <param name="value"></param>
        protected override void OnStringValueChanged(string value)
        {
            if (EnumType is not null && CanConvertFromString(value))
            {
                _value = (Enum)ConvertFromString(value);
                RaisePropertyChanged(nameof(Value));
            }
        }
    }
}
