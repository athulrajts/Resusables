using System.Xml;

namespace KEI.Infrastructure
{
    /// <summary>
    /// PropertyObject implementation for <see cref="int"/>
    /// </summary>
    internal class IntPropertyObject : PropertyObject<int>, INumericPropertyObject
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public IntPropertyObject(string name, int value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => DataObjectType.Integer;

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

            if (Increment is int inc)
            {
                writer.WriteElementString(nameof(Increment), inc.ToString());
            }

            if (Max is int max)
            {
                writer.WriteElementString(nameof(Max), max.ToString());
            }

            if (Min is int min)
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
            if (base.ReadXmlElement(elementName, reader))
            {
                return true;
            }

            if (elementName == nameof(Increment))
            {
                Increment = reader.ReadElementContentAsInt();
                return true;
            }
            else if (elementName == nameof(Max))
            {
                Max = reader.ReadElementContentAsInt();
                return true;
            }
            else if (elementName == nameof(Min))
            {
                Min = reader.ReadElementContentAsInt();
            }

            return false;
        }
    }
}
