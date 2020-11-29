using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KEI.Infrastructure
{
    /// <summary>
    /// Class used internally to create DataObjects
    /// </summary>
    public static class DataObjectFactory
    {
        /// <summary>
        /// Create map for <see cref="DataObject.Type"/> to it's implementation
        /// </summary>
        private static readonly Dictionary<string, Type> factory = new Dictionary<string, Type>
        {
            { "bool" , typeof(BoolDataObject) },
            { "byte" , typeof(ByteDataObject) },
            { "int" , typeof(IntDataObject) },
            { "float" , typeof(FloatDataObject) },
            { "double" , typeof(DoubleDataObject) },
            { "string", typeof(StringDataObject) },
            { "enum" , typeof(EnumDataObject) },
            { "dc" , typeof(ContainerDataObject) },
            { "char", typeof(CharDataObject)}
        };

        /// <summary>
        /// Create map for <see cref="DataObject.Type"/> to it's implementation
        /// Used for creating <see cref="PropertyObject"/>
        /// </summary>
        private static readonly Dictionary<string, Type> propfactory = new Dictionary<string, Type>
        {
            { "bool" , typeof(BoolPropertyObject) },
            { "byte" , typeof(BytePropertyObject) },
            { "int" , typeof(IntPropertyObject) },
            { "float" , typeof(FloatPropertyObject) },
            { "double" , typeof(DoublePropertyObject) },
            { "string", typeof(StringPropertyObject) },
            { "enum" , typeof(EnumPropertyObject) },
            { "opt" , typeof(SelectablePropertyObject) },
            { "dc" , typeof(ContainerPropertyObject) },
            { "char", typeof(CharPropertyObject)}
        };

        /// <summary>
        /// Support for 3rd party implementation for <see cref="DataObject"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RegisterDataObject<T>()
            where T : DataObject
        {
            var instance = (DataObject)FormatterServices.GetUninitializedObject(typeof(T));

            factory.Add(instance.Type, typeof(T));
        }

        /// <summary>
        /// Support for 3rd party implementations for <see cref="PropertyObject"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RegisterPropertyObject<T>()
            where T : PropertyObject
        {
            var instance = (PropertyObject)FormatterServices.GetUninitializedObject(typeof(T));

            propfactory.Add(instance.Type, typeof(T));
        }

        /// <summary>
        /// Gets an uninitialized <see cref="DataObject"/> implementation for given type.
        /// </summary>
        /// <param name="typeid"></param>
        /// <returns></returns>
        public static DataObject GetDataObject(string typeid)
        {
            return factory.ContainsKey(typeid)
                ? (DataObject)FormatterServices.GetUninitializedObject(factory[typeid])
                : null;
        }

        /// <summary>
        /// Gets an uninitialized <see cref="PropertyObject"/> implementation for given type.
        /// </summary>
        /// <param name="typeid"></param>
        /// <returns></returns>
        public static DataObject GetPropertyObject(string typeid)
        {
            return propfactory.ContainsKey(typeid)
                ? (DataObject)FormatterServices.GetUninitializedObject(propfactory[typeid])
                : null;
        }

        /// <summary>
        /// Gets an initialized <see cref="DataObject"/> implementation for given value and name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DataObject GetDataObjectFor(string name, object value)
        {
            return value switch
            {
                byte => new ByteDataObject { Name = name, Value = (byte)value },
                char => new CharDataObject { Name = name, Value = (char)value },
                bool => new BoolDataObject { Name = name, Value = (bool)value },
                int => new IntDataObject { Name = name, Value = (int)value },
                float => new FloatDataObject { Name = name, Value = (float)value },
                double => new DoubleDataObject { Name = name, Value = (double)value },
                string => new StringDataObject { Name = name, Value = (string)value },
                Enum => new EnumDataObject { Name = name, Value = (Enum)value, EnumType = value.GetType() },
                IDataContainer => new ContainerDataObject(name, (IDataContainer)value),
                IList => new ContainerDataObject(name, (IList)value),
                null => throw new NullReferenceException(),
                DataObject => value as DataObject,
                _ => new ContainerDataObject(name, value),
            };
        }

        /// <summary>
        /// Gets an initialized <see cref="PropertyObject"/> implementation for given value and name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PropertyObject GetPropertyObjectFor(string name, object value)
        {
            return value switch
            {
                byte => new BytePropertyObject { Name = name, Value = (byte)value },
                char => new CharPropertyObject { Name = name, Value = (char)value },
                bool => new BoolPropertyObject { Name = name, Value = (bool)value },
                int => new IntPropertyObject { Name = name, Value = (int)value },
                float => new FloatPropertyObject { Name = name, Value = (float)value },
                double => new DoublePropertyObject { Name = name, Value = (double)value },
                string => new StringPropertyObject { Name = name, Value = (string)value },
                Enum => new EnumPropertyObject { Name = name, Value = (Enum)value, EnumType = value.GetType() },
                IDataContainer => new ContainerPropertyObject(name, (IDataContainer)value),
                IList => new ContainerPropertyObject(name, (IList)value),
                null => throw new NullReferenceException(),
                PropertyObject => value as PropertyObject,
                _ => new ContainerPropertyObject(name, value)
            };
        }

    }
}
