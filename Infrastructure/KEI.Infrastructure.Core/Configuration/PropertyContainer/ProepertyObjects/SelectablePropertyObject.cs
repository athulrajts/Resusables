using System;
using System.Xml;
using KEI.Infrastructure.Types;
using System.Collections;

namespace KEI.Infrastructure
{
    public class SelectablePropertyObject : PropertyObject
    {
        const string OPTION_ELEMENT = "Option";

        public override string Type => "opt";

        public Selector Value { get; set; } = new Selector();

        public Type ListType { get; set; }

        public override string StringValue
        {
            get { return Value.SelectedItem; }
            set
            {
                if (Value.SelectedItem == value)
                {
                    return;
                }

                Value.SelectedItem = value;

                RaisePropertyChanged(nameof(StringValue));
            }
        }

        public override EditorType Editor => EditorType.Enum;

        public SelectablePropertyObject(string name, IConvertible value, IList options)
        {
            Name = name;
            Value = new Selector(value.ToString(), options);
        }

        public override object GetValue()
        {
            return Value.SelectedItem;
        }

        public override bool SetValue(object value)
        {
            if (ValidateForType(value.ToString()) == false)
            {
                return false;
            }

            Value.SelectedItem = value.ToString();

            return true;
        }

        public override Type GetDataType()
        {
            return ListType.GetGenericArguments()[0];
        }

        public override bool ValidateForType(string value)
        {
            return Value.Option.Contains(value);
        }

        protected override void WriteXmlInternal(XmlWriter writer)
        {
            base.WriteXmlInternal(writer);

            writer.WriteObjectXML(Value.Type);

            foreach (var opt in Value.Option)
            {
                writer.WriteElementString(OPTION_ELEMENT, opt);
            }
        }

        protected override bool ReadElement(string elementName, XmlReader reader)
        {
            if(elementName == nameof(TypeInfo))
            {
                ListType = reader.ReadObjectXML<TypeInfo>();

                Value.Type = ListType.GetGenericArguments()[0];

                return true;
            }
            else if(elementName == OPTION_ELEMENT)
            {
                Value.Option.Add(reader.ReadElementContentAsString());
                
                return true;
            }

            return false;
        }

    }
}
