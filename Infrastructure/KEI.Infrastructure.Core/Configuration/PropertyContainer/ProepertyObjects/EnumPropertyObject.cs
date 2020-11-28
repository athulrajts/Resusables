using System;
using System.Xml;
using KEI.Infrastructure.Types;

namespace KEI.Infrastructure
{
    public class EnumPropertyObject : PropertyObject<Enum>
    {
        public Type EnumType { get; set; }

        public override EditorType Editor => EditorType.Enum;

        public override string Type => "enum";

        protected override bool ReadElement(string elementName, XmlReader reader)
        {
            if(elementName == nameof(TypeInfo))
            {
                EnumType = reader.ReadObjectXML<TypeInfo>();

                return true;
            }

            return false;
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

        public override Type GetDataType()
        {
            return EnumType;
        }
    }
}
