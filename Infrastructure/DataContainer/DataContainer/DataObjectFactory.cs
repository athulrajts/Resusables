using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace KEI.Infrastructure
{
    public static class DataObjectType
    {
        public const string Boolean = "b";
        public const string Byte = "byte";
        public const string Char = "c";
        public const string Integer = "i";
        public const string Float = "f";
        public const string Enum = "enum";
        public const string Double = "d";
        public const string String = "s";
        public const string File = "file";
        public const string Folder = "folder";
        public const string Selectable = "opt";
        public const string DateTime = "dt";
        public const string TimeSpan = "ts";
        public const string Color = "color";
        public const string Point = "pt";
        public const string Array1D = "array-1";
        public const string Array2D = "array-2";
        public const string Container = "dc";
        public const string Collection = "dcl";
        public const string Xml = "xml";
        public const string Json = "json";
    }


    /// <summary>
    /// Class used internally to create DataObjects
    /// </summary>
    public static class DataObjectFactory
    {
        private static Regex propertyNameRegex = new Regex("[a-zA-Z]+[a-zA-Z0-9_]*", RegexOptions.Compiled);

        /// <summary>
        /// Create mapping for <see cref="DataObject.Type"/> to it's implementation
        /// </summary>
        private static readonly Dictionary<string, Type> typeIdDataObjMapping = new Dictionary<string, Type>
        {
            { DataObjectType.Boolean , typeof(BoolDataObject) },
            { DataObjectType.Byte , typeof(ByteDataObject) },
            { DataObjectType.Integer , typeof(IntDataObject) },
            { DataObjectType.Float , typeof(FloatDataObject) },
            { DataObjectType.Double , typeof(DoubleDataObject) },
            { DataObjectType.String , typeof(StringDataObject) },
            { DataObjectType.Enum , typeof(EnumDataObject) },
            { DataObjectType.Container , typeof(ContainerDataObject) },
            { DataObjectType.Char , typeof(CharDataObject)},
            { DataObjectType.Color , typeof(ColorDataObject)},
            { DataObjectType.Collection , typeof(CollectionDataObject) },
            { DataObjectType.Array1D , typeof(Array1DDataObject)},
            { DataObjectType.Array2D , typeof(Array2DDataObject)},
            { DataObjectType.DateTime , typeof(DateTimeDataObject)},
            { DataObjectType.TimeSpan , typeof(TimeSpanDataObject)},
            { DataObjectType.Point , typeof(PointDataObject)},
            { DataObjectType.Xml , typeof(XmlDataObject) },
            { DataObjectType.Json , typeof(JsonDataObject) }
        };

        /// <summary>
        /// Create mapping for type of data to it's <see cref="DataObject"/> implementation
        /// </summary>
        private static readonly Dictionary<Type, Type> typeDataObjMapping = new Dictionary<Type, Type>
        {
            { typeof(bool) , typeof(BoolDataObject) },
            { typeof(byte) , typeof(ByteDataObject) },
            { typeof(int) , typeof(IntDataObject) },
            { typeof(float) , typeof(FloatDataObject) },
            { typeof(double) , typeof(DoubleDataObject) },
            { typeof(string) , typeof(StringDataObject) },
            { typeof(char), typeof(CharDataObject)},
            { typeof(Color), typeof(ColorDataObject) },
            { typeof(DateTime), typeof(DateTimeDataObject)},
            { typeof(TimeSpan), typeof(TimeSpanDataObject)},
            { typeof(Point), typeof(PointDataObject)}
        };

        /// <summary>
        /// Create mapping for <see cref="DataObject.Type"/> to it's implementation
        /// Used for creating <see cref="PropertyObject"/>
        /// </summary>
        private static readonly Dictionary<string, Type> typeIdPropObjMapping = new Dictionary<string, Type>
        {
            { DataObjectType.Boolean , typeof(BoolPropertyObject) },
            { DataObjectType.Byte , typeof(BytePropertyObject) },
            { DataObjectType.Integer , typeof(IntPropertyObject) },
            { DataObjectType.Float , typeof(FloatPropertyObject) },
            { DataObjectType.Double , typeof(DoublePropertyObject) },
            { DataObjectType.String , typeof(StringPropertyObject) },
            { DataObjectType.Enum , typeof(EnumPropertyObject) },
            { DataObjectType.Selectable , typeof(SelectablePropertyObject) },
            { DataObjectType.Container , typeof(ContainerPropertyObject) },
            { DataObjectType.Char , typeof(CharPropertyObject)},
            { DataObjectType.Color , typeof(ColorPropertyObject)},
            { DataObjectType.Collection , typeof(CollectionPropertyObject)},
            { DataObjectType.File , typeof(FilePropertyObject)},
            { DataObjectType.Folder , typeof(FolderPropertyObject)},
            { DataObjectType.Array1D , typeof(Array1DPropertyObject) },
            { DataObjectType.Array2D , typeof(Array2DPropertyObject) },
            { DataObjectType.DateTime , typeof(DateTimePropertyObject)},
            { DataObjectType.TimeSpan , typeof(TimeSpanPropertyObject)},
            { DataObjectType.Point , typeof(PointPropertyObject)},
            { DataObjectType.Xml , typeof(XmlPropertyObject)},
            { DataObjectType.Json , typeof(JsonPropertyObject)}
        };


        /// <summary>
        /// Create mapping for type of data to it's <see cref="PropertyObject"/> implementation
        /// </summary>
        private static readonly Dictionary<Type, Type> typePropObjMapping = new Dictionary<Type, Type>
        {
            { typeof(bool) , typeof(BoolPropertyObject) },
            { typeof(byte) , typeof(BytePropertyObject) },
            { typeof(int) , typeof(IntPropertyObject) },
            { typeof(float) , typeof(FloatPropertyObject) },
            { typeof(double) , typeof(DoublePropertyObject) },
            { typeof(string), typeof(StringPropertyObject) },
            { typeof(char), typeof(CharPropertyObject)},
            { typeof(Color), typeof(ColorPropertyObject)},
            { typeof(DateTime), typeof(DateTimePropertyObject) },
            { typeof(TimeSpan), typeof(TimeSpanPropertyObject)},
            { typeof(Point), typeof(PointPropertyObject)}
        };

        /// <summary>
        /// Support for 3rd party implementation for <see cref="DataObject"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RegisterDataObject<T>()
            where T : DataObject
        {
            var instance = (DataObject)FormatterServices.GetUninitializedObject(typeof(T));

            typeIdDataObjMapping.Add(instance.Type, typeof(T));
            typeDataObjMapping.Add(instance.GetDataType(), typeof(T));
        }

        /// <summary>
        /// Support for 3rd party implementations for <see cref="PropertyObject"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RegisterPropertyObject<T>()
            where T : PropertyObject
        {
            var instance = (DataObject)FormatterServices.GetUninitializedObject(typeof(T));

            typeIdPropObjMapping.Add(instance.Type, typeof(T));
            typePropObjMapping.Add(instance.GetDataType(), typeof(T));
        }

        /// <summary>
        /// Gets an uninitialized <see cref="DataObject"/> implementation for given type.
        /// </summary>
        /// <param name="typeid"></param>
        /// <returns></returns>
        public static DataObject GetDataObject(string typeid)
        {
            return typeIdDataObjMapping.ContainsKey(typeid)
                ? (DataObject)FormatterServices.GetUninitializedObject(typeIdDataObjMapping[typeid])
                : new NotSupportedDataObject();
        }

        /// <summary>
        /// Gets an uninitialized <see cref="PropertyObject"/> implementation for given type.
        /// </summary>
        /// <param name="typeid"></param>
        /// <returns></returns>
        public static DataObject GetPropertyObject(string typeid)
        {
            return typeIdPropObjMapping.ContainsKey(typeid)
                ? (DataObject)FormatterServices.GetUninitializedObject(typeIdPropObjMapping[typeid])
                : new NotSupportedDataObject();
        }

        /// <summary>
        /// Gets an initialized <see cref="DataObject"/> implementation for given value and name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DataObject GetDataObjectFor(string name, object value, params object[] args)
        {
            Type valueType = value.GetType();

            if (typeDataObjMapping.ContainsKey(valueType))
            {
                object[] constructorArgs;

                if (args is not null && args.Length > 0)
                {
                    constructorArgs = new object[args.Length + 2];
                    constructorArgs[0] = name;
                    constructorArgs[1] = value;

                    for (int i = 0; i < args.Length; i++)
                    {
                        constructorArgs[i + 2] = args[i];
                    }
                }
                else
                {
                    constructorArgs = new object[] { name, value };
                }

                return (DataObject)Activator.CreateInstance(typeDataObjMapping[valueType], constructorArgs);
            }
            else
            {
                /// special objects which can be used for multiple types
                return value switch
                {
                    DataObject data => data,
                    Enum e => new EnumDataObject(name, e),
                    IDataContainer d => new ContainerDataObject(name, d),
                    Array a => GetArrayDataObject(name, a),
                    IList l => new CollectionDataObject(name, l),
                    _ => new ContainerDataObject(name, value), // TODO : give option to choose default ( xml | json | container )
                };
            }

            static DataObject GetArrayDataObject(string name, Array a)
            {
                if (a.GetType().GetElementType().IsPrimitive == false)
                {
                    throw new NotSupportedException("Array of non primitive types not supported");
                }

                return a.Rank switch
                {
                    1 => new Array1DDataObject(name, a),
                    2 => new Array2DDataObject(name, a),
                    _ => throw new NotSupportedException("Array of more than 2 dimensions not supported") // TODO : create data object save n-dimensional array
                };
            }
        }

        /// <summary>
        /// Gets an initialized <see cref="PropertyObject"/> implementation for given value and name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PropertyObject GetPropertyObjectFor(string name, object value, params object[] args)
        {
            if (value is null)
            {
                throw new NullReferenceException();
            }

            Type valueType = value.GetType();

            if (typePropObjMapping.ContainsKey(valueType))
            {
                object[] constructorArgs;

                if (args is not null && args.Length > 0)
                {
                    constructorArgs = new object[args.Length + 2];
                    constructorArgs[0] = name;
                    constructorArgs[1] = value;

                    for (int i = 0; i < args.Length; i++)
                    {
                        constructorArgs[i + 2] = args[i];
                    }
                }
                else
                {
                    constructorArgs = new object[] { name, value };
                }

                return (PropertyObject)Activator.CreateInstance(typePropObjMapping[valueType], constructorArgs);
            }
            else
            {
                /// special objects which can be used for multiple types
                return value switch
                {
                    PropertyObject data => data,
                    Enum e => new EnumPropertyObject(name, e),
                    IDataContainer d => new ContainerPropertyObject(name, d),
                    Array a => GetArrayPropertyObject(name, a),
                    IList l => new CollectionPropertyObject(name, l),
                    _ => new ContainerPropertyObject(name, value), // TODO : give option to choose default ( xml | json | container )
                };
            }

            static PropertyObject GetArrayPropertyObject(string name, Array a)
            {
                if (a.GetType().GetElementType().IsPrimitive == false)
                {
                    throw new NotSupportedException("Array of non primitive types not supported");
                }

                return a.Rank switch
                {
                    1 => new Array1DPropertyObject(name, a),
                    2 => new Array2DPropertyObject(name, a),
                    _ => throw new NotSupportedException("Array of more than 2 dimensions not supported") // TODO : create property object save n-dimensional array
                };
            }
        }

        /// <summary>
        /// Make sure the string is a valid C# identifier
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsValidIdentifierName(string name) => propertyNameRegex.IsMatch(name);

    }
}
