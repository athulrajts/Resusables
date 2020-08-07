using KEI.Infrastructure.Helpers;
using KEI.Infrastructure.Validation;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;
using TypeInfo = KEI.Infrastructure.Types.TypeInfo;

namespace KEI.Infrastructure.Configuration
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
        /// Adds binding
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyKey">name of the <see cref="PropertyObject"/> to bind to</param>
        /// <param name="expression"><see cref="MemberExpression"/> that gets CLR property </param>
        /// <param name="updateSourceOnPropertyChange">Whether or not to update value inside <see cref="PropertyContainer"/> when Target value changes</param>
        public bool SetBinding<T>(string propertyKey, Expression<Func<T>> expression, BindingMode mode = BindingMode.TwoWay)
        {
            var property = FindRecursive(propertyKey);

            if (property == null)
                return false;

            MemberExpression memberExpression = expression.Body as MemberExpression;
            var propinfo = memberExpression.Member as PropertyInfo;

            if (memberExpression is null)
            {
                throw new InvalidCastException("Body of Lambda expression must be a Member expression");
            }

            var target = Expression.Lambda(memberExpression.Expression).Compile().DynamicInvoke();

            if (BindingManager.Instance.GetBinding(target, propinfo.Name) is null)
            {
                var binding = new PropertyBinding(target, property, propinfo, mode);
                if (mode != BindingMode.OneTime)
                {
                    BindingManager.Instance.AddBinding(binding);
                }
            }

            return true;
        }

        /// <summary>
        /// Removes property binding
        /// Could cause memory leaks if not removed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyKey">name of the <see cref="PropertyObject"/> to bind to</param>
        /// <param name="expression"><see cref="MemberExpression"/> that gets CLR property </param>
        public bool RemoveBinding<T>(string propertyKey, Expression<Func<T>> expression)
        {
            var property = FindRecursive(propertyKey);

            if (property == null)
                return false;

            var memberExpression = expression.Body as MemberExpression;
            var propinfo = memberExpression.Member as PropertyInfo;

            if (memberExpression is null)
            {
                throw new InvalidCastException("Body of Lambda expression must be a Member expression");
            }

            var target = Expression.Lambda(memberExpression.Expression).Compile().DynamicInvoke();

            if (BindingManager.Instance.GetBinding(target, propinfo.Name) is PropertyBinding pb)
            {
                BindingManager.Instance.RemoveBinding(pb);
            }

            return true;
        }

        #region IXmlSerializable Memebers

        /// <summary>
        /// Logic for Parsing XML
        /// </summary>
        /// <param name="reader"></param>
        public override void ReadXml(XmlReader reader)
        {
            // We need to for storing the type of a property
            Type typeAttribute = null;

            // Temp object for reading from xml
            var obj = new PropertyObject();

            /// Read <see cref="IDataContainer.Name"/> from XML if it exists
            /// Stored as attribute
            if (reader.HasAttributes)
            {
                reader.MoveToAttribute(0);
                Name = reader.Value;
            }

            // Loop through the Xml stream
            while (reader.Read())
            {
                // Skip if NodeType is not Element
                // No relevant information available here.
                if (reader.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                /// Parse <Property></Property> Tag
                /// Represents a <see cref="PropertyObject"/>
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Property")
                {
                    // Add previously parsing property to the collection
                    // Since we're done with that and starting parsing a new item.
                    if (obj.Value != null)
                    {
                        Add(obj);
                        typeAttribute = null;
                    }

                    // Reinitialize the temporary object.
                    obj = new PropertyObject();

                    /// Read all attribtes of the <Property/> Tag
                    /// 1. Name
                    /// 2. BrowseOption
                    /// 3. Editor
                    /// 4. (Optional) Value
                    /// 5. (Optional) Type
                    /// **Type and Value would get serialized to Attributes instead of elements
                    /// if  <see cref="PropertyObject.Description"/> and <see cref="PropertyObject.Validation"/>
                    /// is null or empty
                    while (reader.MoveToNextAttribute())
                    {
                        /// Read <see cref="DataObject.Name"/>
                        if (reader.Name == nameof(PropertyObject.Name))
                        {
                            obj.Name = reader.Value;
                        }

                        /// Read <see cref="DataObject.Value"/>
                        else if (reader.Name == nameof(PropertyObject.Value))
                        {
                            /// If we already know the type, then convert the string
                            /// value to that type is update <see cref="DataObject.Value"/>"/>
                            if (typeAttribute != null)
                            {
                                obj.Value = typeAttribute.ConvertFrom(reader.Value);
                            }
                            /// Else store the string value and we can conver later when
                            /// we read the Type attribute
                            else
                            {
                                obj.ValueString = reader.Value;
                            }
                        }

                        /// Read Type
                        /// ** Not actually stored inside <see cref="PropertyObject"/> as Property/Field
                        /// just need it to convert it to the right type and then we don't need it anymore.
                        else if (reader.Name == "Type")
                        {
                            /// All Premivitve .NET types can be created by calling
                            /// Type.GetType(System.$(Type.Name))
                            /// or Type.FullName = System.$(Type.Name)
                            typeAttribute = Type.GetType($"System.{reader.Value}") ?? Type.GetType(reader.Value);

                            if (typeAttribute != null)
                            {
                                if (string.IsNullOrEmpty(obj.ValueString) == false)
                                {
                                    obj.Value = typeAttribute.ConvertFrom(obj.ValueString);
                                }
                            }
                        }

                        /// Read <see cref="PropertyObject.BrowseOption"/>
                        else if (reader.Name == nameof(PropertyObject.BrowseOption))
                        {
                            obj.BrowseOption = (BrowseOptions)Enum.Parse(typeof(BrowseOptions), reader.Value);
                        }

                        /// Read <see cref="PropertyObject.Editor"/>
                        else if (reader.Name == nameof(PropertyObject.Editor))
                        {
                            obj.Editor = (EditorType)Enum.Parse(typeof(EditorType), reader.Value);
                        }
                    }
                }

                /// Read <see cref="DataObject.Value"/>
                /// ** Needed when <see cref="DataObject.Value"/> is an <see cref="XmlElement"/> instead of an <see cref="XmlAttribute"/>
                /// When <see cref="PropertyObject.Description"/> and <see cref="PropertyObject.Validation"/> is null or empty
                /// <see cref="DataObject.Value"/> will be serialize as an <see cref="XmlElement"/>
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == nameof(PropertyObject.Value))
                {
                    /// Read All Attributes
                    /// 1. Type
                    /// 2. Value
                    while (reader.MoveToNextAttribute())
                    {
                        /// Read <see cref="DataObject.Value"/>
                        if (reader.Name == nameof(PropertyObject.Value))
                        {
                            // Convert if we already know the type
                            if (typeAttribute != null)
                            {
                                obj.Value = typeAttribute.ConvertFrom(reader.Value);
                            }

                            // Save string value and convert later
                            else
                            {
                                obj.ValueString = reader.Value;
                            }
                        }

                        /// Read Type
                        else if (reader.Name == "Type")
                        {
                            typeAttribute = Type.GetType($"System.{reader.Value}");

                            if (typeAttribute != null)
                            {
                                if (string.IsNullOrEmpty(obj.ValueString) == false)
                                {
                                    obj.Value = typeAttribute.ConvertFrom(obj.ValueString);
                                }
                            }
                        }
                    }
                }

                /// Read <see cref="PropertyObject.Description"/>
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == nameof(PropertyObject.Description))
                {
                    reader.Read();
                    obj.Description = reader.Value;
                }

                /// Read <see cref="PropertyObject.Validation"/>
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == nameof(ValidatorGroup))
                {
                    obj.Validation = reader.ReadObjectXML<ValidatorGroup>();
                }

                /// Read <see cref="DataObject.Value"/> when Value is <see cref="PropertyContainerBase"/>
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "PropertyContainer")
                {
                    // Add previously parsing property to the collection
                    // Since we're done with that and starting parsing a new item.
                    if (obj.Value != null)
                    {
                        Add(obj);
                        obj = new PropertyObject();
                    }

                    var propObj = new PropertyObject();
                    IDataContainer pc = (DataContainerBase)reader.ReadObjectXML(GetType());
                    propObj.Name = pc.Name;
                    propObj.Value = pc;
                    propObj.Editor = EditorType.Object;

                    Add(propObj);
                }

                /// Read <see cref="DataObject.Value"/> when Value is <see cref="Selector"/>
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "EnumProperty")
                {
                    if (obj.Value != null)
                    {
                        Add(obj);
                        obj = new PropertyObject();
                    }

                    var enumProp = new PropertyObject() { Editor = EditorType.Enum };
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
                        else if (reader.Name == nameof(PropertyObject.Name))
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
                    /// That is need to populate a combo box control/>
                    if (count == 0)
                    {
                        selector.Option = new List<string>(Enum.GetNames(selector.Type.GetUnderlyingType()));
                    }

                    // Add it to collection and we're done paring
                    Add(enumProp);
                }

                /// Read <see cref="IDataContainer.UnderlyingType"/>
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == nameof(UnderlyingType))
                {
                    UnderlyingType = reader.ReadObjectXML<TypeInfo>();
                }
            }

            // We're done reading through all of the XML
            // Add last object we were parsing to the list
            if (obj.Value != null)
            {
                Add(obj);
            }
        }


        /// <summary>
        /// Logic for Writing XML
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXml(XmlWriter writer)
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

            /// Write all the <see cref="PropertyObject"/> inside <see cref="IDataContainer.DataCollection"/>
            foreach (PropertyObject item in this)
            {
                /// If <see cref="DataObject.Value"/> is <see cref="PropertyContainerBase"/>
                /// We're recursively call this function again
                if (item.Value is PropertyContainerBase inner)
                {
                    writer.WriteObjectXML(inner);
                }

                /// If <see cref="DataObject.Value"/> is <see cref="Selector"/>
                else if (item.Value is Selector selector)
                {
                    var type = selector.Type.GetUnderlyingType();

                    /// Write <see cref="XmlElement"/> with <see cref="XmlElement.Name"/> as "EnumProperty"
                    /// Need to refactor to avoid hard coded strings
                    writer.WriteStartElement("EnumProperty");

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
                    writer.WriteStartElement("Property");

                    /// Write <see cref="DataObject.Name"/> as <see cref="XmlAttribute"/>
                    writer.WriteAttributeString(nameof(item.Name), item.Name);

                    /// Decide whether we want to write <see cref="DataObject.Value"/>
                    /// as <see cref="XmlElement"/> or <see cref="XmlAttribute"/>
                    if (item.HasOnlyValue())
                    {
                        string typeName = "String";

                        if (item.Value != null)
                        {
                            typeName = item.Value?.GetType().Name;
                        }

                        writer.WriteAttributeString(nameof(item.Value), item.ValueString);
                        writer.WriteAttributeString("Type", typeName);
                    }

                    /// Write <see cref="PropertyObject.BrowseOption"/> as <see cref="XmlAttribute"/>
                    writer.WriteAttributeString(nameof(item.BrowseOption), item.BrowseOption.ToString());

                    /// Write <see cref="PropertyObject.Editor"/> as <see cref="XmlAttribute"/>
                    writer.WriteAttributeString(nameof(item.Editor), item.Editor.ToString());


                    if (item.HasOnlyValue() == false)
                    {
                        writer.WriteStartElement(nameof(item.Value));
                        writer.WriteAttributeString("Type", item.Value.GetType().Name);
                        writer.WriteAttributeString(nameof(item.Value), item.ValueString);
                        writer.WriteEndElement();

                        /// If we have <see cref="PropertyObject.Description"/> write at <see cref="XmlElement"/>
                        if (string.IsNullOrEmpty(item.Description) == false)
                        {
                            writer.WriteElementString(nameof(item.Description), item.Description);
                        }

                        /// If we have <see cref="PropertyObject.Validation"/> write it
                        if (item.Validation != null)
                        {
                            writer.WriteObjectXML(item.Validation);
                        }
                    }

                    // Write End Element for Property
                    writer.WriteEndElement();
                }

            }
        }

        public abstract object Clone();

        #endregion
    }
}
