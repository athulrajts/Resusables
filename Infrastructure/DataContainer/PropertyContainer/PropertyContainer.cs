using System;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Collections.Specialized;
using KEI.Infrastructure.Utils;

namespace KEI.Infrastructure
{
    [XmlRoot("DataContainer")]
    public class PropertyContainer : PropertyContainerBase
    {
        /// <summary>
        /// Storage structure for all data stored inside this object
        /// TODO : Is there a need to use <see cref="System.Collections.Concurrent.ConcurrentDictionary{TKey, TValue}"/> ??
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
            if (IdentifierExtensions.IsValidIdentifier(obj.Name) == false)
            {
                throw new ArgumentException($"{obj.Name} is not a valid c# identifier");
            }

            internalDictionary.Add(obj.Name, obj);

            RaiseCollectionChanged(NotifyCollectionChangedAction.Add, internalDictionary[obj.Name]);
        }


        /// <summary>
        /// Alow collection initliazer using <see cref="Key{T}"/> as key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add<T>(Key<T> key, T value) => Add(key.Name, value);

        /// <summary>
        /// Clear everything
        /// </summary>
        public override void Clear()
        {
            internalDictionary.Clear();
            
            RaiseCollectionChanged(NotifyCollectionChangedAction.Reset, null);
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
            if (IdentifierExtensions.IsValidIdentifier(key) == false)
            {
                throw new ArgumentException($"{key} is not a valid c# identifier");
            }

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
