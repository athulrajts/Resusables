using System;
using System.Collections;
using System.Collections.Generic;

namespace KEI.Infrastructure.Configuration
{
    public static class DataContainerExtensions
    {
        public static EditorType GetEditorType(this Type t)
        {
            if (t == typeof(int) ||
               t == typeof(char) ||
               t == typeof(float) ||
               t == typeof(double) ||
               t == typeof(string))
            {
                return EditorType.String;
            }
            else if (t == typeof(bool))
            {
                return EditorType.Bool;
            }
            else if (t.IsEnum || t == typeof(Selector) )
            {
                return EditorType.Enum;
            }
            else
            {
                return EditorType.Object;
            }
        }

        public static Dictionary<string, DataObject> ToDictionary(this IDataContainer dc)
        {
            var dictionary = new Dictionary<string, DataObject>();

            ToDictionaryInternal(dc, ref dictionary, string.Empty);

            return dictionary;
        }

        private static void ToDictionaryInternal(IDataContainer dc, ref Dictionary<string, DataObject> dictionary, string name)
        {
            foreach (DataObject item in dc)
            {
                if (item.Value is IPropertyContainer dcValue)
                {
                    var nameAppender = string.IsNullOrEmpty(name) ? dcValue.Name : $"{name}.{dcValue.Name}";
                    ToDictionaryInternal(dcValue, ref dictionary, nameAppender);
                }
                else
                {
                    var key = string.IsNullOrEmpty(name) ? item.Name : $"{name}.{item.Name}";
                    dictionary.Add(key, item);
                }

            }
        }


        #region DataContainer Get/Put Extensions

        public static T Get<T>(this IDataContainer dc, string key)
        {
            var retValue = default(T);

            dc.Get(key, ref retValue);

            return retValue;
        }

        public static void Put(this IDataContainer dc, string key, bool value) => dc.Put(key, (object)value);
        public static void Put(this IDataContainer dc, string key, int value) => dc.Put(key, (object)value);
        public static void Put(this IDataContainer dc, string key, float value) => dc.Put(key, (object)value);
        public static void Put(this IDataContainer dc, string key, double value) => dc.Put(key, (object)value);
        public static void Put(this IDataContainer dc, string key, char value) => dc.Put(key, (object)value);
        public static void Put(this IDataContainer dc, string key, string value) => dc.Put(key, (object)value);
        public static void Put(this IDataContainer dc, string key, IDataContainer value) => dc.Put(key, (object)value);

        internal static void Put(this IDataContainer dc, string key, object value)
        {
            if (dc.ContainsProperty(key))
            {
                dc.Set(key, value);
            }
            else
            {
                if (dc is DataContainer d)
                {
                    d.Data.Add(new DataObject
                    {
                        Name = key,
                        Value = value,
                    });
                }
                else if(dc is PropertyContainer p)
                {
                    p.AddProperty(new PropertyObject
                    {
                        Name = key,
                        Value = value,
                        Editor = value.GetType().GetEditorType(),
                    });
                }
            }
        }

        public static DataContainer GetDataContainer(IPropertyContainer pc)
        {
            if (pc == null)
                return null;

            DataContainer dc = new DataContainer();
            dc.Name = pc.Name;

            foreach (var item in pc)
            {
                if (item.Value is IPropertyContainer cpc)
                {
                    dc.Put(item.Name, GetDataContainer(cpc));
                }
                else
                {
                    dc.Data.Add(item);
                }
            }

            return dc;
        }

        #endregion


        public static IDataContainer ToListDataContainer<T>(this IEnumerable<T> list, string name = "UntitledList")
            where T : class
        {
            return DataContainerBuilder.CreateList(name, list);
        }

        public static IDataContainer ToDataContainer<T>(this T obj, string name = "Untitled")
            where T : class
        {
            return DataContainerBuilder.CreateObject(name, obj as IEnumerable);
        }

        public static IPropertyContainer ToPropertyContainer<T>(this T obj, string name = "Untitled")
            where T : class
        {
            return PropertyContainerBuilder.CreateObject(name, obj);
        }

        public static IPropertyContainer ToListPropertyContainer<T>(this IEnumerable<T> list, string name = "UntitledList")
            where T : class
        {
            return PropertyContainerBuilder.CreateList(name, list);
        }


        public static IPropertyContainer ToPropertyContainer(this DataContainer dc) => GetPropertyContainer(dc);
        public static IDataContainer ToDataContainer(this PropertyContainer pc) => GetPropertyContainer(pc);
        private static PropertyContainer GetPropertyContainer(DataContainer dc)
        {
            if (dc == null)
                return null;

            PropertyContainer pc = PropertyContainer.Create();
            pc.Name = dc.Name;

            foreach (var item in dc)
            {
                if (item.Value is DataContainer cdc)
                {
                    pc.Put(item.Name, GetPropertyContainer(cdc));
                }
                else
                {
                    pc.AddProperty(new PropertyObject
                    {
                        Name = item.Name,
                        Value = item.Value,
                        BrowseOption = BrowseOptions.Browsable,
                        Editor = item.Value.GetType().GetEditorType()
                    });
                }
            }

            return pc;
        }
    }
}
