using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        #region Set Operatoins

        /// <summary>
        /// Takes in 2 <see cref="IDataContainer"/> returns a new instance of <see cref="IDataContainer"/>
        /// which contains all the properties in <paramref name="lhs"/> and <paramref name="rhs"/>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"> LHS V RHS</param>
        /// <returns></returns>
        public static IDataContainer Union(this IDataContainer lhs, IDataContainer rhs)
        {
            IDataContainer union = lhs is IPropertyContainer
                ? new PropertyContainer() 
                : new DataContainer();

            union.Name = lhs.Name;

            foreach (DataObject obj in lhs)
            {
                union.Add(obj);
            }

            foreach (DataObject obj in rhs)
            {
                if(union.ContainsData(obj.Name) == false)
                {
                    union.Add(obj);
                }
            }

            return union;
        }

        /// <summary>
        /// Takes in 2 <see cref="IDataContainer"/> returns new instance of <see cref="IDataContainer"/>
        /// which containes properties which are both in LHS and RHS, the values will be from LHS
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns>LHS ^ RHS</returns>
        public static IDataContainer Intersect(this IDataContainer lhs, IDataContainer rhs)
        {
            IDataContainer intersect = lhs is IPropertyContainer
                ? new PropertyContainer()
                : new DataContainer();

            intersect.Name = lhs.Name;

            var lhsKeys = lhs.GetKeys();
            var rhsKeys = rhs.GetKeys();

            var intersectKeys = lhsKeys.Intersect(rhsKeys);

            foreach (var key in intersectKeys)
            {
                intersect.Add(lhs.Find(key));
            }

            return intersect;
        }

        /// <summary>
        /// Takes in 2 <see cref="IDataContainer"/> returns a new instance of <see cref="IDataContainer"/>
        /// Which contains all the properties in LHS and RHS exception the ones that are common to both
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static IDataContainer Difference(this IDataContainer lhs, IDataContainer rhs)
        {
            IDataContainer difference = lhs is IPropertyContainer
                ? new PropertyContainer()
                : new DataContainer();

            difference.Name = lhs.Name;

            var lhsKeys = lhs.GetKeys();
            var rhsKeys = rhs.GetKeys();

            var intersectKeys = lhsKeys.Intersect(rhsKeys);

            foreach (var key in intersectKeys)
            {
                lhs.Remove(lhs.Find(key));
            }

            return difference;
        }

        /// <summary>
        /// Checks whether to <see cref="IDataContainer"/> instances contains same set of properties
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool IsIdentical(this IDataContainer lhs, IDataContainer rhs)
        {
            List<string> lhsKeys = lhs.GetKeys().ToList();
            List<string> rhsKeys = rhs.GetKeys().ToList();

            if(lhsKeys.Count != rhsKeys.Count)
            {
                return false;
            }

            List<string> lhsKeysCopy = new(lhsKeys);
            List<string> rhsKeysCopy = new(rhsKeys);

            foreach (var key in rhsKeys)
            {
                lhsKeysCopy.Remove(key);
            }

            foreach (var key in lhsKeys)
            {
                rhsKeysCopy.Remove(key);
            }

            return lhsKeysCopy.Count == 0 && rhsKeysCopy.Count == 0;
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
