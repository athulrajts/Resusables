using System.Xml;

namespace KEI.Infrastructure
{
    /// <summary>
    /// PropertyObject implementation for <see cref="double"/>
    /// </summary>
    internal class DoublePropertyObject : PropertyObject<double> , INumericPropertyObject
    {
        const double DEFAULT_INCREMENT = 0.1;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public DoublePropertyObject(string name, double value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => "double";

        /// <summary>
        /// Increment for editors
        /// </summary>
        public object Increment { get; set; }

        /// <summary>
        /// Implementation for <see cref="DataObject.CanConvertFromString(string)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool CanConvertFromString(string value)
        {
            return double.TryParse(value, out _);
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.ConvertFromString(string)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFromString(string value)
        {
            return double.TryParse(value, out double tmp)
                ? tmp
                : null;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.WriteXmlContent(XmlWriter)"/>
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlContent(XmlWriter writer)
        {
            base.WriteXmlContent(writer);

            if(Increment is double d && d != DEFAULT_INCREMENT)
            {
                writer.WriteElementString(nameof(Increment), d.ToString());
            }

        }

        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            if(base.ReadXmlElement(elementName, reader))
            {
                return true;
            }

            if(elementName == nameof(Increment))
            {
                Increment = reader.ReadElementContentAsDouble();
            }

            return false;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.OnStringValueChanged(string)"/>
        /// </summary>
        /// <param name="value"></param>
        protected override void OnStringValueChanged(string value)
        {
            if(double.TryParse(value, out _value))
            {
                RaisePropertyChanged(nameof(Value));
            }
        }
    }
}
