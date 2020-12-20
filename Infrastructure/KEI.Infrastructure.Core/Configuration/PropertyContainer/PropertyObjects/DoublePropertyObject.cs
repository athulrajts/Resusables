using System.Xml;

namespace KEI.Infrastructure
{
    /// <summary>
    /// PropertyObject implementation for <see cref="double"/>
    /// </summary>
    internal class DoublePropertyObject : PropertyObject<double> , INumericPropertyObject
    {
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
        public override string Type => DataObjectType.Double;

        /// <summary>
        /// Increment for editors
        /// </summary>
        public object Increment { get; set; }

        /// <summary>
        /// Max value of <see cref="DataObject{T}.Value"/>
        /// </summary>
        public object Max { get; set; }

        /// <summary>
        /// Min value of <see cref="DataObject{T}.Value"/>
        /// </summary>
        public object Min { get; set; }


        /// <summary>
        /// Implementation for <see cref="DataObject.WriteXmlContent(XmlWriter)"/>
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlContent(XmlWriter writer)
        {
            base.WriteXmlContent(writer);

            if(Increment is double inc)
            {
                writer.WriteElementString(nameof(Increment), inc.ToString());
            }

            if(Max is double max)
            {
                writer.WriteElementString(nameof(Max), max.ToString());
            }

            if(Min is double min)
            {
                writer.WriteElementString(nameof(Min), min.ToString());
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
            if(base.ReadXmlElement(elementName, reader))
            {
                return true;
            }

            if(elementName == nameof(Increment))
            {
                Increment = reader.ReadElementContentAsDouble();
                return true;
            }
            else if(elementName == nameof(Max))
            {
                Max = reader.ReadElementContentAsDouble();
                return true;
            }
            else if(elementName == nameof(Min))
            {
                Min = reader.ReadElementContentAsDouble();
            }

            return false;
        }
    }
}
