using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace KEI.Infrastructure
{
    [XmlRoot("DataContainer")]
    public class PropertyContainer : PropertyContainerBase
    {
        /// <summary>
        /// Storage structure for all data stored inside this object
        /// </summary>
        protected readonly Dictionary<string, DataObject> internalDictionary;

        public PropertyContainer()
        {
            internalDictionary = new Dictionary<string, DataObject>();

            CollectionChanged += Data_CollectionChanged;
        }

        /// <summary>
        /// Implementation for <see cref="IDataContainer.Count"/>
        /// </summary>
        public override int Count => internalDictionary.Count;

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
        /// Load state from file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IPropertyContainer FromFile(string path)
        {
            if (XmlHelper.DeserializeFromFile<PropertyContainer>(path) is PropertyContainer dc)
            {
                dc.FilePath = path;

                return dc;
            }

            return null;
        }

        /// <summary>
        /// Function for List initializer
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, object value)
        {
            internalDictionary.Add(key, DataObjectFactory.GetPropertyObjectFor(key, value));

            RaiseCollectionChanged(NotifyCollectionChangedAction.Add, internalDictionary[key]);
        }

        /// <summary>
        /// create clone
        /// </summary>
        /// <returns></returns>
        public override object Clone()
            => XmlHelper.DeserializeFromString<PropertyContainer>(XmlHelper.SerializeToString(this));

        /// <summary>
        /// Implementation for <see cref="IDataContainer.Find(string)"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override DataObject Find(string key) => internalDictionary.ContainsKey(key) ? internalDictionary[key] : null;

        /// <summary>
        /// Implementation for <see cref="IDataContainer.GetEnumerator"/>
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<DataObject> GetEnumerator()
            => internalDictionary.Values.Cast<DataObject>().GetEnumerator();

        /// <summary>
        /// Implementation for <see cref="IDataContainer.GetKeys"/>
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetKeys() => internalDictionary.Keys;

        /// <summary>
        /// Implementation for <see cref="IDataContainer.Remove(DataObject)"/>
        /// </summary>
        /// <param name="obj"></param>
        public override void Remove(DataObject obj) => Remove(obj.Name);

        /// <summary>
        /// Removes item from this
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            if (internalDictionary.ContainsKey(key) == false)
            {
                return;
            }

            var removedItem = internalDictionary[key];

            internalDictionary.Remove(key);

            RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, removedItem);
        }
    }
}
