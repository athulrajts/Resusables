using System;
using System.Xml;
using KEI.Infrastructure.Types;
using System.Collections;
using System.ComponentModel;

namespace KEI.Infrastructure
{
    /// <summary>
    /// Specialized PropertyObject implementation whos value
    /// can on be from a set of valid values.
    /// There is no DataObject implementation this is only used when need to be changed from UI
    /// so a combobox can be used instead of a textbox
    /// </summary>
    internal class SelectablePropertyObject : PropertyObject
    {
        const string OPTION_ELEMENT = "Option";

        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => DataObjectType.Selectable;

        public Selector Value { get; set; } = new Selector();

        /// <summary>
        /// Type of list of valid values
        /// </summary>
        public Type ListType { get; set; }

        /// <summary>
        /// Type of each element in list of valid values
        /// </summary>
        public Type ElementType { get; set; }

        /// <summary>
        /// Implementation for <see cref="DataObject.StringValue"/>
        /// </summary>
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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public SelectablePropertyObject(string name, IConvertible value, IList options)
        {
            Name = name;
            Value = new Selector(value.ToString(), options);
            ListType = options.GetType();
            ElementType = ListType.GenericTypeArguments[0];

            Value.PropertyChanged += Value_PropertyChanged;
        }

        ~SelectablePropertyObject()
        {
            Value.PropertyChanged -= Value_PropertyChanged;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.GetValue"/>
        /// </summary>
        /// <returns></returns>
        public override object GetValue()
        {
            return TypeDescriptor.GetConverter(ElementType).ConvertTo(Value.SelectedItem, ElementType);
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.SetValue(object)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool SetValue(object value)
        {
            if (CanConvertFromString(value.ToString()) == false)
            {
                return false;
            }

            Value.SelectedItem = value.ToString();

            return true;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.GetDataType"/>
        /// </summary>
        /// <returns></returns>
        public override Type GetDataType()
        {
            return ElementType;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.CanConvertFromString(string)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool CanConvertFromString(string value)
        {
            return Value.Option.Contains(value);
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.ConvertFromString(string)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFromString(string value)
        {
            return value;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.WriteXmlContent(XmlWriter)"/>
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlContent(XmlWriter writer)
        {
            base.WriteXmlContent(writer);

            writer.WriteObjectXml(Value.Type);

            foreach (var opt in Value.Option)
            {
                writer.WriteElementString(OPTION_ELEMENT, opt);
            }
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.ReadXmlElement(string, XmlReader)"/>
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            if (base.ReadXmlElement(elementName, reader) == true)
            {
                return true;
            }

            if (elementName == nameof(TypeInfo))
            {
                ListType = reader.ReadObjectXml<TypeInfo>();

                ElementType = Value.Type = ListType.GetGenericArguments()[0];

                return true;
            }
            else if(elementName == OPTION_ELEMENT)
            {
                Value.Option.Add(reader.ReadElementContentAsString());
                
                return true;
            }

            return false;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.OnXmlReadingCompleted"/>
        /// </summary>
        protected override void OnXmlReadingCompleted()
        {
            Value.PropertyChanged += Value_PropertyChanged;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.InitializeObject"/>
        /// </summary>
        protected override void InitializeObject()
        {
            Value = new Selector();
        }

        private void Value_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Selector.SelectedItem))
            {
                RaisePropertyChanged(nameof(Value));
            }
        }

    }
}
