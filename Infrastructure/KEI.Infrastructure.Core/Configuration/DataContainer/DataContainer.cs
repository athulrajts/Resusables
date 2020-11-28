using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace KEI.Infrastructure
{

    public class DataContainer : DataContainerBase, INotifyCollectionChanged
    {
        protected readonly Dictionary<string, object> internalDictionary;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public DataContainer()
        {
            internalDictionary = new Dictionary<string, object>();
        }

        protected virtual object GetValue(string key)
        {
            var data = FindRecursive(key);

            if(data is null)
            {
                throw new KeyNotFoundException($"{key} not found");
            }

            return data.GetValue();
        }

        protected virtual void SetValueInternal(string key, object value)
        {
            var data = FindRecursive(key);

            if(data is not null)
            {
                data.SetValue(value);
            }
        }

        public object this[string key]
        {
            get => GetValue(key.ToString());
            set => SetValue(key.ToString(), value);
        }

        public override int Count => internalDictionary.Count;

        public void Add(string key, object value)
        {
            internalDictionary.Add(key, DataObjectFactory.GetDataObjectFor(key, value));

            var eArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<object> { internalDictionary[key] });

            Data_CollectionChanged(this, eArgs);

            CollectionChanged?.Invoke(this, eArgs);
        }

        public void Add(object key, object value) => Add(key.ToString(), value);

        public void Clear() => internalDictionary.Clear();

        public override bool ContainsProperty(string key)
        {
            var split = key.Split('.');

            if (split.Length == 1)
            {
                return internalDictionary.ContainsKey(key);
            }
            else
            {
                if (internalDictionary[split.First()] is ContainerDataObject cdo)
                {
                    return cdo.Value.ContainsProperty(key);
                }
                else
                {
                    return false;
                }
            }

        }

        public void Remove(string key)
        {
            if(internalDictionary.ContainsKey(key))
            {
                var eArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<object> { internalDictionary[key] });

                Data_CollectionChanged(this, eArgs);

                CollectionChanged?.Invoke(this, eArgs);
            }

            internalDictionary.Remove(key);
        }

        public override IEnumerator<DataObject> GetEnumerator() => internalDictionary.Values.Cast<DataObject>().GetEnumerator();

        public override IEnumerable<string> GetKeys() => internalDictionary.Keys;

        public override void Add(DataObject obj) => Add(obj.Name, obj.GetValue());

        public override void Remove(DataObject obj) => Remove(obj.Name);

        public override DataObject Find(string key) => 
            internalDictionary.ContainsKey(key) 
            ? internalDictionary[key] as DataObject 
            : null;

        /// <summary>
        /// Create <see cref="DataDictionary"/> from XML serialized file
        /// </summary>
        /// <param name="path">Path to XML file</param>
        /// <returns><see cref="DataDictionary"/> deserilized from path</returns>
        public static DataContainer FromFile(string path)
        {
            if (XmlHelper.Deserialize<DataContainer>(path) is DataContainer dc)
            {
                dc.FilePath = path;
                Console.WriteLine("Read");
                return dc;
            }

            return null;
        }
    }
}
