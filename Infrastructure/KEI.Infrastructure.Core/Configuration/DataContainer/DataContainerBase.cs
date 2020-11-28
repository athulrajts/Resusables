using System;
using System.Linq;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;
using Prism.Mvvm;
using KEI.Infrastructure.Types;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml;
using KEI.Infrastructure.Logging;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

namespace KEI.Infrastructure
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
        public string Name { get; set; }


        /// <summary>
        /// The path from which this object was deserialised from
        /// </summary>
        public string FilePath { get; set; }


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

                    if(data is null)
                    {
                        continue;
                    }

                    if (data.GetValue() is IDataContainer dc)
                    {
                        if (dc.UnderlyingType is null)
                        {
                            continue;
                        }

                        if (typeof(IList).IsAssignableFrom(prop.PropertyType))
                        {
                            var list = (IList)Activator.CreateInstance(dc.UnderlyingType);

                            foreach (var listItem in dc)
                            {
                                if (listItem.GetValue() is IDataContainer li)
                                {
                                    list.Add(li.Morph());
                                }
                            }

                            prop.SetValue(result, list);
                        }
                        else
                        {
                            prop.SetValue(result, dc.Morph());
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

                    if (item.GetValue() is IDataContainer dc)
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
            if (actions.Count > 0)
            {
                actions.ForEach(action => action());

                return true;
            }

            return false;
        }

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
                if (workingCopy.ContainsProperty(item.Name) == false)
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
                if (workingBase.ContainsProperty(item.Name) == false)
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

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #region IXmlSerializable Members

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

                // start of a DataObject
                else if (reader.Name == DataObject.START_ELEMENT)
                {
                    // Get DataObject to read.
                    var obj = DataObjectFactory.GetDataObject(reader.GetAttribute(DataObject.TYPE_ID_ATTRIBUTE));

                    if (obj is not null)
                    {
                        using var newReader = XmlReader.Create(new StringReader(reader.ReadOuterXml()));

                        newReader.Read();

                        obj.ReadXml(newReader);

                        Add(obj);
                    }
                }

                // start of ContainerDataObject
                else if (reader.Name == "DataContainer")
                {
                    // get a ContainerDataObject to start reading.
                    var obj = (ContainerDataObject)DataObjectFactory.GetDataObject("dc");

                    if (obj is not null)
                    {
                        obj.Value = new DataContainer();

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
                writer.WriteObjectXML(UnderlyingType);
            }

            // write all the dataobjects
            foreach (var obj in this)
            {
                obj.WriteXml(writer);
            }
        }

        #endregion


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
        /// Adds binding
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
    }


}
