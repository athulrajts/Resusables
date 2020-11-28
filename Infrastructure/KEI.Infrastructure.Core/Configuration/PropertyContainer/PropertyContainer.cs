using KEI.Infrastructure.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Serialization;

namespace KEI.Infrastructure
{
    [XmlRoot("DataContainer")]
    public class PropertyContainer : PropertyContainerBase, INotifyCollectionChanged
    {
        protected readonly Dictionary<string, object> internalDictionary;

        public PropertyContainer()
        {
            internalDictionary = new Dictionary<string, object>();
        }

        public override int Count => internalDictionary.Count;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public override void Add(DataObject obj) => Add(obj.Name, obj);

        public static IPropertyContainer FromFile(string path)
        {
            if (XmlHelper.Deserialize<PropertyContainer>(path) is PropertyContainer dc)
            {
                dc.FilePath = path;

                return dc;
            }

            return null;
        }

        public void Add(string key, object value)
        {
            internalDictionary.Add(key, Transform(key, value));

            var eArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<object> { internalDictionary[key] });

            Data_CollectionChanged(this, eArgs);

            CollectionChanged?.Invoke(this, eArgs);
        }

        private DataObject Transform(string key, object value)
        {
            if (value is DataObject) return value as DataObject;

            return DataObjectFactory.GetPropertyObjectFor(key, value) ?? DataObjectFactory.GetDataObjectFor(key, value);
        }

        public override object Clone()
            => XmlHelper.DeserializeFromString<PropertyContainer>(XmlHelper.Serialize(this));

        public override DataObject Find(string key) => internalDictionary.ContainsKey(key) ? internalDictionary[key] as DataObject : null;

        public override IEnumerator<DataObject> GetEnumerator()
            => internalDictionary.Values.Cast<DataObject>().GetEnumerator();

        public override IEnumerable<string> GetKeys() => internalDictionary.Keys;

        public override void Remove(DataObject obj) => Remove(obj.Name);

        public void Remove(string key)
        {
            if (internalDictionary.ContainsKey(key))
            {
                var eArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<object> { internalDictionary[key] });

                Data_CollectionChanged(this, eArgs);

                CollectionChanged?.Invoke(this, eArgs);
            }

            internalDictionary.Remove(key);
        }
    }
}
