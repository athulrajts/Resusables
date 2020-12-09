using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Dynamic;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Runtime.CompilerServices;
using KEI.Infrastructure.Logging;
using System.Collections.ObjectModel;

namespace KEI.Infrastructure
{
    /// <summary>
    /// Base class for <see cref="DataContainer"/> and <see cref="PropertyContainer"/>
    /// </summary>
    [XmlRoot("DataContainer")]
    public abstract class DataContainerBase : DynamicObject, IDataContainer, ICustomTypeDescriptor, IXmlSerializable, IEnumerable
    {
        #region Properties

        /// <summary>
        /// Name of the DataContainer
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The path from which this object was deserialised from
        /// </summary>
        public string FilePath { get; internal set; }

        /// <summary>
        /// Represents Type that this instance can be cast into
        /// can be converted into
        /// </summary>
        public Types.TypeInfo UnderlyingType { get; set; }

        /// <summary>
        /// Number of data objects in this container
        /// </summary>
        public abstract int Count { get; }

        #endregion

        #region Manipulation

        /// <summary>
        /// Gets Data Value from a DataContainer object
        /// </summary>
        /// <typeparam name="T">Type of result</typeparam>
        /// <param name="key">Key, which uniquely identifies the data</param>
        /// <param name="value">Value object passed as reference</param>
        /// <returns>Is Success</returns>
        public virtual bool GetValue<T>(string key, ref T value)
        {
            var data = FindRecursive(key);

            bool result = false;

            if (data is null)
            {
                Logger.Warn($"Unable to find \"{key}\" from {Name}");
            }
            else if (data.GetValue() is T val)
            {
                value = val;
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Sets the Data value
        /// </summary>
        /// <param name="key">Key, which uniquely identifies the data</param>
        /// <param name="value">Value to set</param>
        public virtual void SetValue(string key, object value)
        {
            var data = FindRecursive(key);

            if (data is null)
            {
                Logger.Warn($"Unable to find \"{key}\" from {Name}");
            }
            else
            {
                data.SetValue(value);
            }
        }

        /// <summary>
        /// Merge two DataContainers
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public bool Merge(IDataContainer right)
        {
            List<Action> actions = new List<Action>();

            AddNewItems(this, right, ref actions);

            RemoveOldItems(this, right, ref actions);

            // Store updated config if changes were made.
            if (actions.Any())
            {
                actions.ForEach(action => action());

                return true;
            }

            return false;
        }

        /// <summary>
        /// Serializes DataContainer object to an XML file to the given path
        /// </summary>
        /// <param name="path">file path to store the config</param>
        /// <returns>Is Sucess</returns>
        public virtual bool Store(string path)
        {
            FilePath = path;

            if (XmlHelper.SerializeToFile(this, path) == false)
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
        public virtual bool ContainsData(string key) => Find(key) is DataObject;

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
                GetValue(split.First(), ref temp);

                if (temp is IDataContainer dc)
                {
                    return dc.Find(string.Join(".", split.Skip(1)));
                }
                else
                {
                    throw new Exception("Nested configs should be of type IDataContainer");
                }
            }
        }

        #endregion

        #region Morphing

        /// <summary>
        /// Maps this contents of this objects to a class objected
        /// represented by <see cref="UnderlyingType"/>
        /// </summary>
        /// <returns>returns as object</returns>
        public virtual object Morph()
        {
            if (UnderlyingType is null)
            {
                throw new InvalidOperationException("Morph not supported for this instance");
            }

            Type morphType = UnderlyingType;

            var result = Activator.CreateInstance(morphType);

            foreach (var prop in morphType.GetProperties())
            {
                if (prop.CanWrite)
                {
                    var data = Find(prop.Name);

                    if (data is null)
                    {
                        continue;
                    }

                    if (data.GetValue() is IDataContainer dc)
                    {
                        if (dc.UnderlyingType is null)
                        {
                            continue;
                        }
                        
                        prop.SetValue(result, dc.Morph());
                    }
                    else if(data.Type == "dcl" && data.GetValue() is ObservableCollection<IDataContainer> oc)
                    {
                        var listType = prop.PropertyType;
                        
                        if (listType is not null)
                        {
                            var list = (IList)Activator.CreateInstance(listType);

                            foreach (var item in oc)
                            {
                                list.Add(item.Morph());
                            }

                            prop.SetValue(result, list);
                        }
                    }
                    else if (data is not null)
                    {
                        prop.SetValue(result, data.GetValue());
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Maps this contents of this container to a class objected
        /// represented by <see cref="UnderlyingType"/>
        /// </summary>
        /// <returns></returns>
        public virtual IList MorphList()
        {
            if (UnderlyingType is null)
            {
                throw new InvalidOperationException("Morph not supported for this instance");
            }

            Type morphType = UnderlyingType;

            var list = Activator.CreateInstance(morphType) as IList;

            if (typeof(IList).IsAssignableFrom(morphType))
            {
                foreach (var item in this)
                {
                    if (item is ContainerDataObject cdo)
                    {
                        list.Add(cdo.Value.Morph());
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Generic wrapper the casts call to <see cref="MorphList"/> to give type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IList<T> MorphList<T>() => (IList<T>)MorphList();


        /// <summary>
        /// Maps this contents of this objects to a class objected
        /// represented by <see cref="UnderlyingType"/>
        /// </summary>
        /// <typeparam name="T">Returns as specified type</typeparam>
        /// <returns></returns>
        public T Morph<T>() => (T)Morph();

        #endregion

        #region Abstract Functions

        /// <summary>
        /// Adds DataObject to datacontainer
        /// </summary>
        /// <param name="obj"></param>
        public abstract void Add(DataObject obj);

        /// <summary>
        /// Removes DataObject from datacontainer
        /// </summary>
        /// <param name="name"></param>
        public abstract void Remove(DataObject name);

        /// <summary>
        /// Gets a DataObject from this datacontainer
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract DataObject Find(string key);

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

        #endregion

        #region Binding Members

        /// <summary>
        /// Adds binding
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyKey">name of the <see cref="DataObject"/> to bind to</param>
        /// <param name="expression"><see cref="MemberExpression"/> that gets CLR property </param>
        /// <param name="updateSourceOnPropertyChange">Whether or not to update value inside <see cref="DataContainer"/> when Target value changes</param>
        public bool SetBinding<T>(string propertyKey, Expression<Func<T>> expression, BindingMode mode = BindingMode.TwoWay)
        {
            var property = FindRecursive(propertyKey);

            if (property is null)
            {
                return false;
            }

            MemberExpression memberExpression = expression.Body as MemberExpression;
            var propinfo = memberExpression.Member as PropertyInfo;

            if (memberExpression is null)
            {
                throw new InvalidCastException("Body of Lambda expression must be a Member expression");
            }

            var target = Expression.Lambda(memberExpression.Expression).Compile().DynamicInvoke();

            if (BindingManager.Instance.GetBinding(target, propinfo.Name) is null)
            {
                var binding = new DataObjectBinding(target, property, propinfo, mode);
                if (mode != BindingMode.OneTime)
                {
                    BindingManager.Instance.AddBinding(binding);
                }
            }

            return true;
        }

        /// <summary>
        /// Adds binding based on convention, Uses the name of bound property in VM as key in IDataContainer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"><see cref="MemberExpression"/> that gets CLR property </param>
        /// <param name="updateSourceOnPropertyChange">Whether or not to update value inside <see cref="DataContainer"/> when Target value changes</param>
        public bool SetBinding<T>(Expression<Func<T>> expression, BindingMode mode = BindingMode.TwoWay)
        {
            MemberExpression memberExpression = expression.Body as MemberExpression;
            var propinfo = memberExpression.Member as PropertyInfo;

            if (memberExpression is null)
            {
                throw new InvalidCastException("Body of Lambda expression must be a Member expression");
            }

            var property = FindRecursive(propinfo.Name);

            if (property is null)
            {
                return false;
            }

            var target = Expression.Lambda(memberExpression.Expression).Compile().DynamicInvoke();

            if (BindingManager.Instance.GetBinding(target, propinfo.Name) is null)
            {
                var binding = new DataObjectBinding(target, property, propinfo, mode);
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

            if (property is null)
            {
                return false;
            }

            var memberExpression = expression.Body as MemberExpression;
            var propinfo = memberExpression.Member as PropertyInfo;

            if (memberExpression is null)
            {
                throw new InvalidCastException("Body of Lambda expression must be a Member expression");
            }

            var target = Expression.Lambda(memberExpression.Expression).Compile().DynamicInvoke();

            if (BindingManager.Instance.GetBinding(target, propinfo.Name) is DataObjectBinding pb)
            {
                BindingManager.Instance.RemoveBinding(pb);
            }

            return true;
        }

        /// <summary>
        /// Removes binding based on convention, Uses the name of bound property in VM as key in IDataContainer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool RemoveBinding<T>(Expression<Func<T>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            var propinfo = memberExpression.Member as PropertyInfo;

            if (memberExpression is null)
            {
                throw new InvalidCastException("Body of Lambda expression must be a Member expression");
            }

            var property = FindRecursive(propinfo.Name);

            if (property is null)
            {
                return false;
            }

            var target = Expression.Lambda(memberExpression.Expression).Compile().DynamicInvoke();

            if (BindingManager.Instance.GetBinding(target, propinfo.Name) is DataObjectBinding pb)
            {
                BindingManager.Instance.RemoveBinding(pb);
            }

            return true;
        }

        #endregion

        #region DynamicObect Overrides

        /// <summary>
        /// Called by WPF binding to get value of binding
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            // Check if it's an actual member
            bool ret = base.TryGetMember(binder, out result);

            // else It's a dynamic member
            if (ret == false)
            {
                /// find <see cref="DataObject"/> corresponding to the binding
                if (Find(binder.Name) is DataObject data)
                {
                    // get value
                    result = data.GetValue();

                    ret = true;
                }
                // don't have member
                else
                {
                    ret = false;
                }
            }

            return ret;
        }

        /// <summary>
        /// Called by WPF binding to set value of a binding
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            // Check if it's an actual member
            bool ret = base.TrySetMember(binder, value);

            // else It's a dynamic member
            if (ret == false)
            {
                /// find <see cref="DataObject"/> corresponding to the binding
                if (Find(binder.Name) is DataObject data)
                {
                    // set value
                    data.SetValue(value);

                    ret = true;
                }
                // don't have member
                else
                {
                    ret = false;
                }
            }

            return ret;
        }

        /// <summary>
        /// Implementation for <see cref="DynamicObject.GetDynamicMemberNames"/>
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return GetKeys();
        }

        #endregion

        #region IXmlSerializable Members

        protected virtual DataObject GetUnitializedDataObject(string type)
        {
            return DataObjectFactory.GetDataObject(type);
        }

        public XmlSchema GetSchema() => null;

        public virtual void ReadXml(XmlReader reader)
        {
            // Read attribute name
            if (reader.GetAttribute(DataObject.KEY_ATTRIBUTE) is string s)
            {
                Name = s;
            }

            // read to content
            reader.Read();

            while (reader.EOF == false)
            {
                // nothing of value skip.
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                }

                // Start of Underlying type
                // for DataObjects created from taking CLR objects as reference
                else if (reader.Name == nameof(Types.TypeInfo))
                {
                    UnderlyingType = reader.ReadObjectXml<Types.TypeInfo>();
                }
                
                // We're reading a DataObject implementation
                else
                {
                    string dataObjectType = reader.GetAttribute(DataObject.TYPE_ID_ATTRIBUTE);

                    /// Get uninitialized Object based on <see cref="DataObject.TYPE_ID_ATTRIBUTE"/>
                    if(GetUnitializedDataObject(dataObjectType) is DataObject obj)
                    {
                        /// need to create a new XmlReader so that <see cref="DataObject"/> implementation
                        /// can read till end.
                        using var newReader = XmlReader.Create(new StringReader(reader.ReadOuterXml()));

                        // move to content
                        newReader.Read();

                        obj.ReadXml(newReader);

                        if (obj is not NotSupportedDataObject)
                        {
                            Add(obj);
                        }
                    }
                }    

            }
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            // write name as attribute if we have a name
            if (string.IsNullOrEmpty(Name) == false)
            {
                writer.WriteAttributeString(DataObject.KEY_ATTRIBUTE, Name);
            }

            // write underlying type if this was created from a .NET object
            if (UnderlyingType is not null)
            {
                writer.WriteObjectXml(UnderlyingType);
            }

            // write all the dataobjects
            foreach (var obj in this)
            {
                obj.WriteXml(writer);
            }
        }

        #endregion

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, object changedItem)
            => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, changedItem));

        #endregion

        #region INotifyPropertyChanged Memmbers

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string property = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value) == true)
            {
                return false;
            }

            storage = value;

            RaisePropertyChanged(property);

            return true;
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Implementation for <see cref="IEnumerable.GetEnumerator"/>
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region ICustomTypeDescriptor Members

        /// <summary>
        /// Implementation for <see cref="ICustomTypeDescriptor.GetComponentName"/>
        /// </summary>
        /// <returns></returns>
        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        /// <summary>
        /// Implementation for <see cref="ICustomTypeDescriptor.GetDefaultEvent"/>
        /// </summary>
        /// <returns></returns>
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        /// <summary>
        /// Implementation for <see cref="ICustomTypeDescriptor.GetClassName"/>
        /// </summary>
        /// <returns></returns>
        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        /// <summary>
        /// Implementation for <see cref="ICustomTypeDescriptor.GetEvents(Attribute[])"/>
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        /// <summary>
        /// Implementation for <see cref="ICustomTypeDescriptor.GetEvents"/>
        /// </summary>
        /// <returns></returns>
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        /// <summary>
        /// Implementation for <see cref="ICustomTypeDescriptor.GetConverter"/>
        /// </summary>
        /// <returns></returns>
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        /// <summary>
        /// Implementation for <see cref="ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor)"/>
        /// </summary>
        /// <param name="pd"></param>
        /// <returns></returns>
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        /// <summary>
        /// Imlplementation for <see cref="ICustomTypeDescriptor.GetAttributes"/>
        /// </summary>
        /// <returns></returns>
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        /// <summary>
        /// Implementation for <see cref="ICustomTypeDescriptor.GetEditor(Type)"/>
        /// </summary>
        /// <param name="editorBaseType"></param>
        /// <returns></returns>
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        /// <summary>
        /// Implementation for <see cref="ICustomTypeDescriptor.GetDefaultProperty"/>
        /// </summary>
        /// <returns></returns>
        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        /// <summary>
        /// Implementation for <see cref="ICustomTypeDescriptor.GetProperties"/>
        /// </summary>
        /// <returns></returns>
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(Array.Empty<Attribute>());
        }

        /// <summary>
        /// Implementation for <see cref="ICustomTypeDescriptor.GetProperties(Attribute[])"/>
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            ArrayList properties = new ArrayList();
            
            foreach (DataObject data in this)
            {
                var attrs = new List<Attribute>();

                // add attributes for property grid
                if (data is PropertyObject po)
                {
                    // add description attribute
                    if (string.IsNullOrEmpty(po.Description) == false)
                    {
                        attrs.Add(new DescriptionAttribute(po.Description));
                    }
                    
                    // add display name attribute
                    if (string.IsNullOrEmpty(po.DisplayName) == false)
                    {
                        attrs.Add(new DisplayNameAttribute(po.DisplayName));
                    }

                    // add category attribute
                    if (string.IsNullOrEmpty(po.Category) == false)
                    {
                        attrs.Add(new CategoryAttribute(po.Category));
                    }

                    // add browsable/readonly attributes
                    attrs.Add(po.BrowseOption switch
                    {
                        BrowseOptions.Browsable => new BrowsableAttribute(true),
                        BrowseOptions.NonBrowsable => new BrowsableAttribute(false),
                        BrowseOptions.NonEditable => new ReadOnlyAttribute(true),
                        _ => throw new NotImplementedException()
                    });

                    /// add editor attribute
                    /// custom editors should be registered using <see cref="CustomUITypeEditorMapping.RegisterEditor{TEditor}(string)"/>
                    if (CustomUITypeEditorMapping.GetEditorType(po.GetType()) is Type t)
                    {
                        attrs.Add(new EditorAttribute(t, t));
                    }

                    /// add type converter attribute
                    /// custom type converters should be registered using <see cref="CustomUITypeEditorMapping.RegisterConverter{TConverter}(string)"/>
                    if (CustomUITypeEditorMapping.GetConverterType(po.GetType()) is Type ct)
                    {
                        attrs.Add(new TypeConverterAttribute(ct));
                    }

                }

                /// Create property descriptor
                properties.Add(new DataObjectPropertyDescriptor(data, attrs.ToArray()));
            }

            PropertyDescriptor[] props = (PropertyDescriptor[])properties.ToArray(typeof(PropertyDescriptor));

            return new PropertyDescriptorCollection(props);
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// helper methods to merge datacontainers
        /// </summary>
        /// <param name="workingCopy">left</param>
        /// <param name="workingBase">right</param>
        /// <param name="addActions"></param>
        private void AddNewItems(IDataContainer workingCopy, IDataContainer workingBase, ref List<Action> addActions)
        {
            foreach (var item in workingBase)
            {
                if (workingCopy.ContainsData(item.Name) == false)
                {
                    addActions.Add(() => workingCopy.Add(item));

                    Logger.Info($"Added new property {item.Name} = {item.StringValue}");
                }
                else if (item is ContainerDataObject baseChild)
                {
                    AddNewItems(workingCopy.Get<IDataContainer>(item.Name), baseChild.Value, ref addActions);
                }

            }
        }


        /// <summary>
        /// helper methods to merge datacontainers
        /// </summary>
        /// <param name="workingCopy">left</param>
        /// <param name="workingBase">right</param>
        /// <param name="addActions"></param>
        private void RemoveOldItems(IDataContainer workingCopy, IDataContainer workingBase, ref List<Action> removeActions)
        {
            foreach (var item in workingCopy)
            {
                if (workingBase.ContainsData(item.Name) == false)
                {
                    removeActions.Add(() => workingCopy.Remove(item));

                    Logger.Info($"Removed property {item.Name} = {item.StringValue}");
                }
                else if (item is ContainerDataObject baseChild)
                {
                    RemoveOldItems(workingCopy.Get<IDataContainer>(item.Name), baseChild.Value, ref removeActions);
                }
            }
        }

        #endregion

        #region Bubble PropertyChanged Notifications

        /// <summary>
        /// Attaches PropertyChanged listeners to newly added Properties
        /// Removes PropertyChanged listeners from removed Properties
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems.Cast<DataObject>())
                {
                    item.PropertyChanged += OnPropertyChangedRaised;

                    if (item.GetValue() is IDataContainer dc)
                    {
                        dc.PropertyChanged += OnPropertyChangedRaised;
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems.Cast<DataObject>())
                {
                    item.PropertyChanged -= OnPropertyChangedRaised;

                    if (item.GetValue() is IDataContainer dc)
                    {
                        dc.PropertyChanged -= OnPropertyChangedRaised;
                    }
                }
            }
        }

        private void OnPropertyChangedRaised(object sender, PropertyChangedEventArgs e)
        {
            string propName = e.PropertyName;

            /// If sender is one of our children, pass the event on up the tree.
            if (sender is DataObject o && e.PropertyName == "Value")
            {
                RaisePropertyChanged(o.Name);
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

    }

}
