using System.IO;
using System.Xml;
using KEI.Infrastructure.Validation;

namespace KEI.Infrastructure
{
    public abstract class PropertyContainerBase : DataContainerBase, IPropertyContainer
    {
        /// <summary>
        /// Set <see cref="PropertyObject.BrowseOption"/> of the <see cref="PropertyObject"/>
        /// identified by name.
        /// </summary>
        /// <param name="property">name of <see cref="PropertyObject"/> to update BrowseOption</param>
        /// <param name="option"></param>
        public bool SetBrowseOptions(string property, BrowseOptions option)
        {
            if (FindRecursive(property) is PropertyObject di)
            {
                di.BrowseOption = option;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Set <see cref="PropertyObject.Validation"/> of the <see cref="PropertyObject"/>
        /// identified by name.
        /// </summary>
        /// <param name="property">name of <see cref="PropertyObject"/> to update Validation</param>
        /// <param name="option"></param>
        public bool SetValidation(string property, ValidatorGroup validation)
        {
            if (FindRecursive(property) is PropertyObject di)
            {
                di.Validation = validation;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Implementation for <see cref="DataContainerBase.ReadXml(XmlReader)"/>
        /// </summary>
        /// <param name="reader"></param>
        public override void ReadXml(XmlReader reader)
        {
            // Read attribute name
            if (reader.GetAttribute(DataObject.KEY_ATTRIBUTE) is string s)
            {
                Name = s;
            }

            reader.Read();

            while (reader.EOF == false)
            {
                // nothing of value skip.
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                }

                // start of a PropertyObject
                else if (reader.Name == DataObject.START_ELEMENT)
                {
                    // Get PropertyObject to read.
                    var obj = DataObjectFactory.GetPropertyObject(reader.GetAttribute(DataObject.TYPE_ID_ATTRIBUTE));

                    if (obj is not null)
                    {
                        using var newReader = XmlReader.Create(new StringReader(reader.ReadOuterXml()));

                        newReader.Read();
                        
                        obj.ReadXml(newReader);

                        Add(obj);
                    }
                }

                // start of ContainerPropertyObject
                else if (reader.Name == "DataContainer")
                {
                    var obj = (ContainerPropertyObject)DataObjectFactory.GetPropertyObject("dc");

                    if (obj is not null)
                    {
                        obj.Value = new PropertyContainer();

                        using var newReader = XmlReader.Create(new StringReader(reader.ReadOuterXml()));

                        newReader.Read();

                        obj.ReadXml(newReader);

                        Add(obj);
                    }
                }

                // Start of Underlying type
                // for Datacontainers created from .NET objects
                else if (reader.Name == nameof(Types.TypeInfo))
                {
                    UnderlyingType = reader.ReadObjectXML<Types.TypeInfo>();
                }
            }
        }

        public abstract object Clone();

    }
}
