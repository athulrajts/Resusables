using KEI.Infrastructure.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Serialization;

namespace KEI.Infrastructure.Configuration
{
    [XmlRoot("PropertyContainer")]
    public class PropertyDictionary : PropertyContainerBase, INotifyCollectionChanged
    {
        protected readonly Dictionary<string, object> internalDictionary;

        public PropertyDictionary()
        {
            internalDictionary = new Dictionary<string, object>();
        }

        public override int Count => internalDictionary.Count;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public override void Add(DataObject obj) => Add(obj.Name, obj);

        public static IPropertyContainer FromFile(string path)
        {
            if (XmlHelper.Deserialize<PropertyDictionary>(path) is PropertyDictionary dc)
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

        private PropertyObject Transform(string key, object value)
        {
            if(value is PropertyObject)
            {
                return value as PropertyObject;
            }

            PropertyObject obj = new PropertyObject
            {
                Name = key,
                Editor = value.GetType().GetEditorType()
            };

            if (value is Enum e)
            {
                obj.Value = new Selector(e);
            }
            else if (value.GetType().IsPrimitiveType() ||
                value is IPropertyContainer ||
                value is Selector)
            {
                obj.Value = value;
            }
            else if(value is IList l)
            {
                obj.Value = PropertyContainerBuilder.CreateList(key, l as IEnumerable<object>);
            }
            else
            {
                obj.Value = PropertyContainerBuilder.CreateObject(key, value);
            }

            return obj;
        }

        public override object Clone()
            => XmlHelper.DeserializeFromString<PropertyDictionary>(XmlHelper.Serialize(this));

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
