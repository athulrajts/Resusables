using System;
using System.Collections.Generic;
using System.IO;

namespace KEI.Infrastructure
{
    public static class DataContainerExtensions
    {
        public static Dictionary<string, DataObject> ToFlatDictionary(this IDataContainer dc)
        {
            var dictionary = new Dictionary<string, DataObject>();

            ToFlatDictionaryInternal(dc, ref dictionary, string.Empty);

            return dictionary;
        }

        private static void ToFlatDictionaryInternal(IDataContainer dc, ref Dictionary<string, DataObject> dictionary, string name)
        {
            foreach (DataObject item in dc)
            {
                if (item.GetValue() is IDataContainer idc)
                {
                    var nameAppender = string.IsNullOrEmpty(name) ? idc.Name : $"{name}.{idc.Name}";
                    ToFlatDictionaryInternal(idc, ref dictionary, nameAppender);
                }
                else
                {
                    var key = string.IsNullOrEmpty(name) ? item.Name : $"{name}.{item.Name}";
                    dictionary.Add(key, item);
                }

            }
        }

        #region DataContainer Get/Put Extensions

        public static T GetValue<T>(this IDataContainer dc, string key)
        {
            if(dc is null)
            {
                throw new NullReferenceException();
            }

            var retValue = default(T);

            dc.GetValue(key, ref retValue);

            return retValue;
        }

        public static void PutValue(this IDataContainer dc, string key, object value)
        {
            if(dc is null)
            {
                throw new NullReferenceException();
            }

            if (dc.ContainsData(key))
            {
                dc.SetValue(key, value);
            }
            else
            {
                if (dc is IPropertyContainer)
                {
                    dc.Add(DataObjectFactory.GetPropertyObjectFor(key, value));
                }
                else
                {
                    dc.Add(DataObjectFactory.GetDataObjectFor(key, value));
                }
            }
        }

        #endregion

        public static void WriteBytes(this IDataContainer container, Stream stream)
        {
            var writer = new BinaryWriter(stream);

            foreach (var data in container)
            {
                if (data is IWriteToBinaryStream wbs)
                {
                    wbs.WriteBytes(writer);
                }
            }

        }
    }
}
