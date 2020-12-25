using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace KEI.Infrastructure
{
    /// <summary>
    /// Concrete implementation for <see cref="IDataContainer"/>
    /// </summary>
    [XmlRoot("DataContainer")]
    public class DataContainer : DataContainerBase
    {
        /// <summary>
        /// Storage structure for all data stored inside this object
        /// </summary>
        protected readonly Dictionary<string, DataObject> internalDictionary;

        public DataContainer()
        {
            internalDictionary = new Dictionary<string, DataObject>();

            CollectionChanged += Data_CollectionChanged;
        }

        /// <summary>
        /// Implementation for <see cref="IDataContainer.Count"/>
        /// </summary>
        public override int Count => internalDictionary.Count;

        /// <summary>
        /// Function to make use of list initializers
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, object value)
        {
            internalDictionary.Add(key, DataObjectFactory.GetDataObjectFor(key, value));

            RaiseCollectionChanged(NotifyCollectionChangedAction.Add, internalDictionary[key]);
        }

        /// <summary>
        /// Clear all data
        /// </summary>
        public void Clear() => internalDictionary.Clear();

        /// <summary>
        /// Implementation for <see cref="IDataContainer.ContainsData(string)"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override bool ContainsData(string key)
        {
            var split = key.Split('.');

            if (split.Length == 1)
            {
                return internalDictionary.ContainsKey(key);
            }
            else
            {
                if (internalDictionary[split.First()].GetValue() is IDataContainer dc)
                {
                    return dc.ContainsData(key);
                }
                else
                {
                    return false;
                }
            }

        }

        /// <summary>
        /// Remove property with name <paramref name="key"/>
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            if(internalDictionary.ContainsKey(key) == false)
            {
                return;
            }

            var removedItem = internalDictionary[key];

            internalDictionary.Remove(key);

            RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, removedItem);
        }

        /// <summary>
        /// Implementation for <see cref="IDataContainer.GetEnumerator"/>
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<DataObject> GetEnumerator() => internalDictionary.Values.Cast<DataObject>().GetEnumerator();

        /// <summary>
        /// Implementation for <see cref="IDataContainer.GetKeys"/>
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetKeys() => internalDictionary.Keys;

        /// <summary>
        /// Implementation for <see cref="IDataContainer.Add(DataObject)"/>
        /// </summary>
        /// <param name="obj"></param>
        public override void Add(DataObject obj)
        {
            internalDictionary.Add(obj.Name, obj);

            RaiseCollectionChanged(NotifyCollectionChangedAction.Add, internalDictionary[obj.Name]);
        }

        /// <summary>
        /// Implementation for <see cref="IDataContainer.Remove(DataObject)"/>
        /// </summary>
        /// <param name="obj"></param>
        public override void Remove(DataObject obj) => Remove(obj.Name);

        /// <summary>
        /// Implemenation for <see cref="IDataContainer.Find(string)"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override DataObject Find(string key) => 
            internalDictionary.ContainsKey(key) 
            ? internalDictionary[key] 
            : null;

        /// <summary>
        /// Create <see cref="DataDictionary"/> from XML serialized file
        /// </summary>
        /// <param name="path">Path to XML file</param>
        /// <returns><see cref="DataDictionary"/> deserilized from path</returns>
        public static DataContainer FromFile(string path)
        {
            if (XmlHelper.DeserializeFromFile<DataContainer>(path) is DataContainer dc)
            {
                dc.FilePath = path;
                return dc;
            }

            return null;
        }
    }
}
