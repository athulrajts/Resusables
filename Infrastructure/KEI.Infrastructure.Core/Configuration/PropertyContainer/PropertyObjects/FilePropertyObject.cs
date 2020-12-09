using System;
using System.Collections.Generic;
using System.Xml;

namespace KEI.Infrastructure
{
    public record Filter(string Description, string Extenstion);
    public class FilterCollection : List<Filter> { }

    public interface IFileProperty
    {
        public FilterCollection Filters { get; }
    }

    /// <summary>
    /// PropertyObject implementation for storing file paths
    /// There is no corresponding DataObject implementation, since the only
    /// Difference between <see cref="StringPropertyObject"/> <see cref="FilePropertyObject"/>
    /// is the Editor in PropertyGrid. <see cref="StringDataObject"/> should be used for storing filepaths
    /// in <see cref="DataContainer"/>
    /// </summary>
    internal class FilePropertyObject : StringPropertyObject , IFileProperty
    {
        const string FILTER_DESCRIPTION_ATTRIBUTE = "desc";
        const string FILTER_EXTENSTION_ATTRIBUTE = "ext";
        const string FILTER_ELEMENT = "Filter";

        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => "file";

        /// <summary>
        /// Used by the editor, which should be a file browse dialog
        /// <see cref="Tuple{T1, T2}.Item1"/> is Description of filter
        /// <see cref="Tuple{T1, T2}.Item2"/> is Extension of filter
        /// </summary>
        public FilterCollection Filters { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="filters"></param>
        public FilePropertyObject(string name, string value): base(name, value)
        {
            Filters = new FilterCollection();
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.WriteXmlContent(XmlWriter)"/>
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlContent(XmlWriter writer)
        {
            base.WriteXmlContent(writer);

            foreach (var filter in Filters)
            {
                writer.WriteStartElement(FILTER_ELEMENT);
                writer.WriteAttributeString(FILTER_DESCRIPTION_ATTRIBUTE, filter.Description);
                writer.WriteAttributeString(FILTER_EXTENSTION_ATTRIBUTE, filter.Extenstion);
                writer.WriteEndElement();
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

            if(elementName == FILTER_ELEMENT)
            {
                string desc = reader.GetAttribute(FILTER_DESCRIPTION_ATTRIBUTE);
                string ext = reader.GetAttribute(FILTER_EXTENSTION_ATTRIBUTE);

                Filters.Add(new(desc, ext));

                reader.Read();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.InitializeObject"/>
        /// </summary>
        protected override void InitializeObject()
        {
            Filters = new();
        }
    }
}
