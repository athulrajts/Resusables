using KEI.Infrastructure.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Serialization;

namespace KEI.Infrastructure.Configuration
{

    [XmlRoot(nameof(DataContainer))]
    public class DataDictionary : DataContainerBase, INotifyCollectionChanged
    {
        protected readonly Dictionary<string, object> internalDictionary;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public DataDictionary()
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

            if (data.Value is Selector s)
            {
                var type = s.Type.GetUnderlyingType();
                if (type.IsEnum)
                {
                    return Enum.Parse(type, s.SelectedItem);
                }
                else
                {
                    return type.ConvertFrom(s.SelectedItem);
                }
            }
            else
            {
                return (internalDictionary[key.ToString()] as DataObject).Value;
            }
        }

        protected virtual void SetValue(string key, object value)
        {
            if (value is null)
            {
                throw new ArgumentNullException("Value cannot be null");
            }

            var type = value.GetType();
            
            var currentValue = FindRecursive(key);

            if (type != currentValue.Type)
            {
                throw new ArgumentException("New value type doesn't match existing values type");
            }

            if (type == typeof(DataDictionary))
            {
                if ((value as DataDictionary).UnderlyingType.FullName != (currentValue.Value as DataDictionary).UnderlyingType.FullName)
                {
                    throw new ArgumentException("New value type doesn't match existing values type");
                }
            }

            if (currentValue.Value is Selector s)
            {
                currentValue.Value = s.Clone(value.ToString());
            }
            else
            {
                currentValue.Value = value;
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
            internalDictionary.Add(key, Transform(key, value));

            var eArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<object> { internalDictionary[key] });

            Data_CollectionChanged(this, eArgs);

            CollectionChanged?.Invoke(this, eArgs);
        }

        private DataObject Transform(string key, object value)
        {
            if(value is DataObject)
            {
                return value as DataObject;
            }

            DataObject obj = new DataObject { Name = key };

            if (value is Enum e)
            {
                obj.Value = new Selector(e);
            }
            else if (value.GetType().IsPrimitiveType() ||
                value is IDataContainer ||
                value is Selector)
            {
                obj.Value = value;
            }
            else if (value is IList l)
            {
                obj.Value = DataContainerBuilder.CreateList(key, l as IEnumerable<object>);
            }
            else
            {
                obj.Value = DataContainerBuilder.CreateObject(key, value);
            }

            return obj;
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
                if (internalDictionary[split.First()] is DataDictionary dd)
                {
                    return dd.ContainsProperty(key);
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

        public override void Add(DataObject obj) => Add(obj.Name, obj.Value);

        public override void Remove(DataObject obj) => Remove(obj.Name);

        public override DataObject Find(string key) => internalDictionary.ContainsKey(key) ? internalDictionary[key] as DataObject : null;

        /// <summary>
        /// Create <see cref="DataDictionary"/> from XML serialized file
        /// </summary>
        /// <param name="path">Path to XML file</param>
        /// <returns><see cref="DataDictionary"/> deserilized from path</returns>
        public static DataDictionary FromFile(string path)
        {
            if (XmlHelper.Deserialize<DataDictionary>(path) is DataDictionary dc)
            {
                dc.FilePath = path;
                Console.WriteLine("Read");
                return dc;
            }

            return null;
        }
    }
}
