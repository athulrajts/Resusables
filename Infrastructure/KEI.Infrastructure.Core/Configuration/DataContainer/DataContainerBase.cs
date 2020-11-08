using System;
using System.Linq;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;
using Prism.Mvvm;
using KEI.Infrastructure.Types;
using KEI.Infrastructure.Helpers;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml;

namespace KEI.Infrastructure.Configuration
{
    /// <summary>
    /// Base class for <see cref="DataContainer"/> and <see cref="PropertyContainer"/>
    /// </summary>
    public abstract class DataContainerBase : BindableBase, IDataContainer, IXmlSerializable, IEnumerable
    {
        #region Properties

        /// <summary>
        /// Name of the DataContainer
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }


        /// <summary>
        /// The path from which this object was deserialised from
        /// </summary>
        [XmlIgnore]
        public string FilePath { get; set; }


        /// <summary>
        /// Represents Type that this instance can be cast into
        /// can be converted into
        /// </summary>
        [XmlElement(IsNullable = false)]
        public TypeInfo UnderlyingType { get; set; }

        public abstract int Count { get; }

        #endregion


        /// <summary>
        /// Gets Data Value from a DataContainer object
        /// </summary>
        /// <typeparam name="T">Type of result</typeparam>
        /// <param name="key">Key, which uniquely identifies the data</param>
        /// <param name="value">Value object passed as reference</param>
        /// <returns>Is Success</returns>
        public virtual bool Get<T>(string key, ref T value)
        {
            var split = key.Split('.');

            if (split.Length == 1)
            {
                var dataItem = Find(key);

                if (dataItem == null)
                {
                    ViewService.Warn($"Unable to find \"{key}\" from {Name}");
                    return false;
                }

                if (dataItem.Value is Selector s)
                {
                    var type = s.Type.GetUnderlyingType();
                    if (type.IsEnum)
                    {
                        value = (T)Enum.Parse(type, s.SelectedItem);
                    }
                    else
                    {
                        value = (T)type.ConvertFrom(s.SelectedItem);
                    }
                }
                else
                {
                    value = (T)dataItem.Value;
                }

                return true;
            }
            else
            {
                object temp = null;
                Get(split.First(), ref temp);
                if (temp is IDataContainer dc)
                {
                    dc.Get(string.Join(".", split.Skip(1)), ref value);
                }
                else
                {
                    throw new Exception("Nested configs should be of type PropertyContainer");
                }
            }

            return false;
        }
     
        /// <summary>
        /// Sets the Data value
        /// </summary>
        /// <param name="key">Key, which uniquely identifies the data</param>
        /// <param name="value">Value to set</param>
        public virtual void Set(string key, object value)
        {
            var split = key.Split('.');

            if (split.Length == 1)
            {
                var dataItem = Find(key);

                if (dataItem == null)
                {
                    ViewService.Warn($"Unable to find \"{key}\" from {Name}");
                    return;
                }

                if (dataItem.Value is Selector s)
                {
                    dataItem.Value = s.Clone(value?.ToString());
                }
                else if (dataItem.Value is IDataContainer dc)
                {
                    if (dc.UnderlyingType == null)
                        return;

                    if (value is IDataContainer dcValue)
                    {
                        dataItem.Value = dcValue;
                    }

                }
                else
                {
                    dataItem.Value = value;
                }
            }
            else
            {
                object temp = null;
                Get(split.First(), ref temp);

                if (temp is IDataContainer dc)
                {
                    dc.Set(string.Join(".", split.Skip(1)), value);
                }
                else
                {
                    throw new Exception("Nested configs should be of type IDataContainer");
                }
            }
        }

        /// <summary>
        /// Serializes DataContainer object to an XML file to the given path
        /// </summary>
        /// <param name="path">file path to store the config</param>
        /// <returns>Is Sucess</returns>
        public virtual bool Store(string path)
        {
            FilePath = path;

            if (XmlHelper.Serialize(this, path) == false)
            {
                ViewService.Warn($"Unable to Write config \"{Name}\" ");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Stores the config file in the path <see cref="FilePath"/>
        /// </summary>
        /// <returns></returns>
        public bool Store() => Store(FilePath);

        /// <summary>
        /// Checks if the Object contains data with given key
        /// </summary>
        /// <param name="key">data Key to search for</param>
        /// <returns>Is Found</returns>
        public virtual bool ContainsProperty(string key) => Find(key) is DataObject;


        /// <summary>
        /// Maps this contents of this objects to a class objected
        /// represented by <see cref="UnderlyingType"/>
        /// </summary>
        /// <returns>returns as object</returns>
        public virtual object Morph()
        {
            if (UnderlyingType == null)
                return null;

            var morphType = UnderlyingType.GetUnderlyingType();

            var result = Activator.CreateInstance(morphType);

            foreach (var prop in morphType.GetProperties())
            {
                if (prop.CanWrite)
                {
                    if (Find(prop.Name) is DataObject di)
                    {
                        if (di.Value is IConvertible)
                        {
                            prop.SetValue(result, di.Value);
                        }
                        else if (di.Value is IDataContainer cfg)
                        {
                            if (cfg.UnderlyingType == null)
                                continue;

                            var cfgMorphType = cfg.UnderlyingType.GetUnderlyingType();

                            if (typeof(IList).IsAssignableFrom(cfgMorphType))
                            {
                                var list = Activator.CreateInstance(cfgMorphType) as IList;

                                foreach (var item in cfg)
                                {
                                    if (item.Value is IDataContainer objectCfg)
                                    {
                                        list.Add(objectCfg.Morph());
                                    }
                                }

                                prop.SetValue(result, list);
                            }
                            else
                            {
                                prop.SetValue(result, cfg.Morph());
                            }
                        }
                        else if (di.Value is Selector s)
                        {
                            prop.SetValue(result, Enum.Parse(s.Type.GetUnderlyingType(), s.SelectedItem));
                        }
                    }

                }
            }

            return result;
        }

        public virtual object MorphList()
        {
            if (UnderlyingType == null)
                return null;

            var morphType = UnderlyingType.GetUnderlyingType();

            if (morphType == null)
                return null;

            var list = Activator.CreateInstance(morphType) as IList;

            if (typeof(IList).IsAssignableFrom(morphType))
            {
                foreach (var item in this)
                {
                    if (item.Value is IDataContainer objectCfg)
                    {
                        list.Add(objectCfg.Morph());
                    }
                }
            }

            return list;
        }

        public List<T> MorphList<T>() => (List<T>)MorphList();


        /// <summary>
        /// Maps this contents of this objects to a class objected
        /// represented by <see cref="UnderlyingType"/>
        /// </summary>
        /// <typeparam name="T">Returns as specified type</typeparam>
        /// <returns></returns>
        public T Morph<T>() => (T)Morph();

        /// <summary>
        /// Allows <see cref="IDataContainer"/> to be used in foreach loop
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator<DataObject> GetEnumerator();


        /// <summary>
        /// Get all keys in this object
        /// Is not recursives, will not provide keys for child <see cref="IDataContainer"/>
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<string> GetKeys();

        #region Bubble Up Property Changed Notifications from Children

        /// <summary>
        /// Attaches PropertyChanged listeners to newly added Properties
        /// Removes PropertyChanged listeners from removed Properties
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Data_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems.Cast<DataObject>())
                {
                    item.PropertyChanged += OnPropertyChangedRaised;

                    if (item.Value is IDataContainer dc)
                    {
                        dc.PropertyChanged += OnPropertyChangedRaised;
                    }
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems.Cast<DataObject>())
                {
                    item.PropertyChanged -= OnPropertyChangedRaised;

                    if (item.Value is IDataContainer dc)
                    {
                        dc.PropertyChanged -= OnPropertyChangedRaised;
                    }
                }
            }
        }

        private void OnPropertyChangedRaised(object sender, PropertyChangedEventArgs e)
        {
            string propName = e.PropertyName;

            /// We only care when <see cref="DataObject.Name"/> is raised as event
            if (propName == nameof(DataObject.Value) ||
                propName == nameof(DataObject.ValueString))
            {
                return;
            }

            /// If sender is one of our children, pass the event on up the tree.
            if (sender is DataObject)
            {
                RaisePropertyChanged(propName);
            }

            /// If one of our childrens value is <see cref="IDataContainer"/>
            /// We need to handle propertychanged for our grandchildren also
            else if (sender is IDataContainer dc)
            {
                var split = propName.Split('.');

                if (split.FirstOrDefault() == dc.Name)
                {
                    RaisePropertyChanged(propName);
                }
                else
                {
                    //Create full name, $(parent).$(child)
                    RaisePropertyChanged($"{dc.Name}.{propName}");
                }

            }
        }

        #endregion

        public bool Merge(IDataContainer right)
        {
            List<Action> actions = new List<Action>();

            AddNewItems(this, right, ref actions);

            RemoveOldItems(this , right, ref actions);

            // Store updated config if changes were made.
            if (actions.Count > 0)
            {
                actions.ForEach(action => action());

                return true;
            }

            return false;
        }

        private void AddNewItems(IDataContainer workingCopy, IDataContainer workingBase, ref List<Action> addActions)
        {
            foreach (var item in workingBase)
            {
                if (workingCopy.ContainsProperty(item.Name) == false)
                {
                    addActions.Add(() => workingCopy.Add(item));

                    Logging.Logger.Info($"Added new property {item.Name} = {item.ValueString}");
                }
                else if (item.Value is IDataContainer baseChild)
                {
                    AddNewItems(workingCopy.Get<IDataContainer>(item.Name), baseChild, ref addActions);
                }

            }
        }

        private void RemoveOldItems(IDataContainer workingCopy, IDataContainer workingBase, ref List<Action> removeActions)
        {
            foreach (var prop in workingCopy)
            {
                if (workingBase.ContainsProperty(prop.Name) == false)
                {
                    removeActions.Add(() => workingCopy.Remove(prop));

                    Logging.Logger.Info($"Removed property {prop.Name} = {prop.ValueString}");
                }
                else if (prop.Value is IDataContainer baseChild)
                {
                    RemoveOldItems(workingCopy.Get<PropertyContainer>(prop.Name), baseChild, ref removeActions);
                }
            }
        }

        public abstract void Add(DataObject obj);
        public abstract void Remove(DataObject name);
        public abstract DataObject Find(string key);
        
        /// <summary>
        /// Gets <see cref="PropertyObject"/> with given name
        /// </summary>
        /// <param name="key">name of <see cref="PropertyObject"/></param>
        /// <returns>reference of <see cref="PropertyObject"/></returns>
        public DataObject FindRecursive(string key)
        {
            var split = key.Split('.');

            if (split.Length == 1)
            {
                return Find(key);
            }
            else
            {
                object temp = null;
                Get(split.First(), ref temp);
                if (temp is DataContainerBase dc)
                {
                    return dc.Find(string.Join(".", split.Skip(1)));
                }
                else
                {
                    throw new Exception("Nested configs should be of type PropertyContainer");
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        public XmlSchema GetSchema() => null;

        public virtual void ReadXml(XmlReader reader)
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
                        Add(dataObj);
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
                        else if (reader.Name == "Type")
                        {
                            typeAttribute = Type.GetType($"System.{reader.Value}");

                            if (typeAttribute != null)
                            {
                                if (string.IsNullOrEmpty(dataObj.ValueString) == false)
                                {
                                    dataObj.Value = typeAttribute.ConvertFrom(dataObj.ValueString);
                                }
                            }
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "DataContainer")
                {
                    // Add previously parsing property to the collection
                    // Since we're done with that and starting parsing a new item.
                    if (dataObj.Value != null)
                    {
                        Add(dataObj);
                        typeAttribute = null;
                    }

                    var dcObj = new DataObject();

                    IDataContainer dc = (IDataContainer)reader.ReadObjectXML(GetType());
                    dcObj.Name = dc.Name;
                    dcObj.Value = dc;
                    Add(dcObj);
                }
                /// Read <see cref="DataObject.Value"/> when Value is <see cref="Selector"/>
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "EnumData")
                {
                    if (dataObj.Value != null)
                    {
                        Add(dataObj);
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
            if (dataObj.Value != null)
            {
                Add(dataObj);
            }
        }

        public virtual void WriteXml(XmlWriter writer)
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

            foreach (var item in this)
            {

                /// If <see cref="DataObject.Value"/> is <see cref="DataContainerBase"/>
                /// We're recursively call this function again
                if (item.Value is DataContainerBase inner)
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
