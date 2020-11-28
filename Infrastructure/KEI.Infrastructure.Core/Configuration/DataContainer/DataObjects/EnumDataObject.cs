using KEI.Infrastructure.Types;
using System;
using System.IO;
using System.Xml;

namespace KEI.Infrastructure
{
    public class EnumDataObject : DataObject<Enum>, IWriteToBinaryStream
    {
        public override string Type => "enum";

        public Type EnumType { get; set; }

        protected override bool ReadElement(string elementName, XmlReader reader)
        {
            bool read = false;

            if (elementName == nameof(TypeInfo))
            {
                EnumType = reader.ReadObjectXML<TypeInfo>();

                read = true;
            }

            return read;
        }

        protected override void OnReadingCompleted()
        {
            Value = (Enum)Enum.Parse(EnumType, StringValue);
        }

        protected override void WriteXmlInternal(XmlWriter writer)
        {
            base.WriteXmlInternal(writer);

            writer.WriteObjectXML(new TypeInfo(EnumType));
        }

        protected override void OnStringValueChanged(string value)
        {
            if (EnumType is not null &&
                ValidateForType(value))
            {
                _value = (Enum)Enum.Parse(EnumType, StringValue);
                RaisePropertyChanged(nameof(Value));
            }
        }

        public override bool ValidateForType(string value)
        {
            return Enum.TryParse(EnumType, value, out _);
        }

        public void WriteBytes(BinaryWriter writer)
        {
            writer.Write(Convert.ToInt32(Value));
        }

        public override Type GetDataType()
        {
            return EnumType;
        }
    }
}
