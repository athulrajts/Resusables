using System;
using System.Linq;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;
using Prism.Mvvm;
using KEI.Infrastructure.Types;
using KEI.Infrastructure.Helpers;
using System.ComponentModel;

namespace KEI.Infrastructure.Configuration
{
    /// <summary>
    /// Base class for <see cref="DataContainer"/> and <see cref="PropertyContainer"/>
    /// </summary>
    public abstract class DataContainerBase : BindableBase, IDataContainer
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

        public abstract IReadOnlyCollection<DataObject> DataCollection { get; }

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
                var dataItem = DataCollection.FirstOrDefault(x => x.Name == key);

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
                var dataItem = DataCollection.FirstOrDefault(x => x.Name == key);

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
        public virtual bool ContainsProperty(string key) => DataCollection.FirstOrDefault(x => x.Name == key) is DataObject;


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
                    if (DataCollection.FirstOrDefault(x => x.Name == prop.Name) is DataObject di)
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

                                foreach (var item in cfg.DataCollection)
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
                            prop.SetValue(result, Enum.Parse(prop.PropertyType, s.SelectedItem));
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
                foreach (var item in DataCollection)
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
        public virtual IEnumerator<DataObject> GetEnumerator() => DataCollection.GetEnumerator();


        /// <summary>
        /// Get all keys in this object
        /// Is not recursives, will not provide keys for child <see cref="IDataContainer"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetKeys() => DataCollection.Select(x => x.Name);

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
    }
}
