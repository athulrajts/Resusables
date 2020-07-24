using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;
using KEI.Infrastructure.Helpers;
using KEI.Infrastructure.Types;

namespace KEI.Infrastructure.Configuration
{
    /// <summary>
    /// Generic class for storing and retrieving data which is capable of storing complex data
    /// structures using a set of primitive types.
    /// </summary>
    public class DataContainer : DataContainerBase, IXmlSerializable
    {
        #region Properties

        /// <summary>
        /// Contains a collection of values which are under this need
        /// Even though any type of date can be stored in this collection
        /// only primitive types are recommented here
        /// </summary>
        internal DataObjectCollection Data { get; set; } = new DataObjectCollection();

        [XmlIgnore]
        public override IReadOnlyCollection<DataObject> DataCollection => Data;

        #endregion

        /// <summary>
        /// Create <see cref="DataContainer"/> from XML serialized file
        /// </summary>
        /// <param name="path">Path to XML file</param>
        /// <returns><see cref="DataContainer"/> deserilized from path</returns>
        public static DataContainer FromFile(string path)
        {
            if (XmlHelper.Deserialize<DataContainer>(path) is DataContainer dc)
            {
                dc.FilePath = path;

                return dc;
            }

            return null;
        }


        /// <summary>
        /// Serializes DataContainer object to an XML file to the given path
        /// </summary>
        /// <param name="path">file path to store the config</param>
        /// <returns>Is Sucess</returns>
        public override bool Store(string path)
        {
            FilePath = path;

            try
            {
                XmlHelper.Serialize(this, path);
            }
            catch (Exception)
            {
                ViewService.Warn($"Unable to Write config \"{Name}\" ");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Creates a clone of this config
        /// </summary>
        /// <returns>Clone</returns>
        public DataContainer Clone()
            => XmlHelper.DeserializeFromString<DataContainer>(XmlHelper.Serialize(this));


        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            // We need to for storing the type of a property
            Type typeAttribute = null;

            // Temp object for reading from xml
            var dataObj = new DataObject();

            /// Read <see cref="IDataContainer.Name"/> from XML if it exists
            /// Stored as attribute
            if (reader.HasAttributes)
            {
                reader.MoveToAttribute(0);
                Name = reader.Value;
            }


            while (reader.Read())
            {
                // Skip if NodeType is not Element
                // No relevant information available here.
                if (reader.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Data")
                {

                    // Add previously parsing property to the collection
                    // Since we're done with that and starting parsing a new item.
                    if (dataObj.Value != null)
                    {
                        Data.Add(dataObj);
                        typeAttribute = null;
                    }

                    dataObj = new DataObject();


                    while (reader.MoveToNextAttribute())
                    {

                        if (reader.Name == nameof(DataObject.Name))
                        {
                            dataObj.Name = reader.Value;
                        }
                        else if (reader.Name == nameof(DataObject.Value))
                        {
                            if (typeAttribute != null)
                            {
                                dataObj.Value = typeAttribute.ConvertFrom(reader.Value);
                            }
                            else
                            {
                                dataObj.ValueString = reader.Value;
                            }
                        }
                        else if(reader.Name == "Type")
                        {
                            typeAttribute = Type.GetType($"System.{reader.Value}");

                            if(typeAttribute != null)
                            {
                                if(string.IsNullOrEmpty(dataObj.ValueString) == false)
                                {
                                    dataObj.Value = typeAttribute.ConvertFrom(dataObj.ValueString);
                                }
                            }
                        }
                    }
                }
                else if(reader.NodeType == XmlNodeType.Element && reader.Name == nameof(DataContainer))
                {
                    // Add previously parsing property to the collection
                    // Since we're done with that and starting parsing a new item.
                    if (dataObj.Value != null)
                    {
                        Data.Add(dataObj);
                        typeAttribute = null;
                    }

                    var dcObj = new DataObject();

                    var dc = reader.ReadObjectXML<DataContainer>();
                    dcObj.Name = dc.Name;
                    dcObj.Value = dc;
                    Data.Add(dcObj);
                }
                /// Read <see cref="DataObject.Value"/> when Value is <see cref="Selector"/>
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "EnumData")
                {
                    if (dataObj.Value != null)
                    {
                        Data.Add(dataObj);
                        dataObj = new DataObject();
                    }

                    var enumProp = new DataObject();
                    var selector = new Selector();
                    int count = 0;

                    /// Read all Attributes
                    /// 1. Name
                    /// 2. Value
                    /// 3. (Optional) Count
                    /// ** Count needed when <see cref="Selector"/> object is not
                    /// based on a <see cref="Enum"/> Type
                    while (reader.MoveToNextAttribute())
                    {
                        /// Read <see cref="Selector.SelectedItem"/>
                        if (reader.Name == "Value")
                        {
                            selector.SelectedItem = reader.Value;
                        }
                        /// Read <see cref="DataObject.Name"/>
                        else if (reader.Name == nameof(DataObject.Name))
                        {
                            enumProp.Name = reader.Value;
                        }
                        /// Only needed to parse xml
                        /// This could probably be removed later.
                        else if (reader.Name == "Count")
                        {
                            count = XmlConvert.ToInt32(reader.Value);
                        }
                    }

                    /// If count == 0 we are dealing with <see cref="Enum"/> based Selector
                    if (count == 0)
                    {
                        enumProp.Value = selector;
                    }
                    /// Else we need also read the allowed values
                    else
                    {
                        for (int i = 0; i < count; i++)
                        {
                            reader.ReadToFollowing("Option");
                            reader.Read();
                            selector.Option.Add(reader.Value);
                        }
                        enumProp.Value = selector;
                    }

                    // need to skip to move to the correct node.
                    reader.Read();

                    /// Read <see cref="Selector.Type"/>
                    selector.Type = reader.ReadObjectXML<TypeInfo>();

                    /// Fill the Allowed types for <see cref="Enum"/> based Selector
                    /// That is need to populate <see cref="System.Windows.Controls.ComboBox"/>
                    if (count == 0)
                    {
                        selector.Option = new List<string>(Enum.GetNames(selector.Type.GetUnderlyingType()));
                    }

                    // Add it to collection and we're done paring
                    Data.Add(enumProp);
                }

                /// Read <see cref="IDataContainer.UnderlyingType"/>
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == nameof(UnderlyingType))
                {
                    UnderlyingType = reader.ReadObjectXML<TypeInfo>();
                }
            }
            
            // We're done reading through all of the XML
            // Add last object we were parsing to the list
            if (dataObj.Value != null)
            {
                Data.Add(dataObj);
            }
        }

        public void WriteXml(XmlWriter writer)
        {

            /// if we have a <see cref="IDataContainer.Name"/>
            /// Write it as an <see cref="XmlAttribute"/>
            if (string.IsNullOrEmpty(Name) == false)
            {
                writer.WriteAttributeString(nameof(Name), Name);
            }

            /// if we have an <see cref="IDataContainer.UnderlyingType"/>
            /// Write it as an <see cref="XmlAttribute"/>
            if (UnderlyingType != null)
            {
                writer.WriteObjectXML(UnderlyingType);
            }

            foreach (var item in Data)
            {

                /// If <see cref="DataObject.Value"/> is <see cref="PropertyContainer"/>
                /// We're recursively call this function again
                if (item.Value is DataContainer inner)
                {
                    writer.WriteObjectXML(inner);
                }
                /// If <see cref="DataObject.Value"/> is <see cref="Selector"/>
                else if (item.Value is Selector selector)
                {
                    var type = selector.Type.GetUnderlyingType();

                    /// Write <see cref="XmlElement"/> with <see cref="XmlElement.Name"/> as "EnumProperty"
                    /// Need to refactor to avoid hard coded strings
                    writer.WriteStartElement("EnumData");

                    /// Write <see cref="DataObject.Name"/> as <see cref="XmlAttribute"/>
                    writer.WriteAttributeString(nameof(item.Name), item.Name);

                    /// If we are not dealing with <see cref="Enum"/> based <see cref="Selector"/>
                    /// write the number of allowed items also to make parsing easier
                    if (type.IsEnum == false)
                    {
                        writer.WriteAttributeString("Count", selector.Option.Count.ToString());
                    }

                    /// Write <see cref="Selector.SelectedItem"/> as Value <see cref="XmlAttribute"/>
                    writer.WriteAttributeString("Value", selector.SelectedItem);

                    /// Write all the allowed values in <see cref="Selector.Option"/>
                    if (type.IsEnum == false)
                    {
                        foreach (var op in selector.Option)
                        {
                            writer.WriteElementString("Option", op);
                        }
                    }

                    /// Write type of elements in <see cref="Selector.Option"/>
                    writer.WriteObjectXML(selector.Type);

                    /// Write the End Element for "EnumProperty"
                    writer.WriteEndElement();
                }
                /// We are dealing with a <see cref="PropertyObject"/> who <see cref="DataObject.Value"/> can be
                /// Directly converted to and from <see cref="string"/> type
                else
                {
                    /// Write Start Element
                    /// Need to refactor Hard coded strings
                    writer.WriteStartElement("Data");

                    /// Write <see cref="DataObject.Name"/> as <see cref="XmlAttribute"/>
                    writer.WriteAttributeString(nameof(item.Name), item.Name);

                    string typeName = "String";
                    if (item.Value != null)
                    {
                        typeName = item.Value?.GetType().Name;
                    }

                    writer.WriteAttributeString(nameof(item.Value), item.ValueString);
                    writer.WriteAttributeString("Type", typeName);

                    // Write End Element for Property
                    writer.WriteEndElement();
                }

            }
        }
    }
}
