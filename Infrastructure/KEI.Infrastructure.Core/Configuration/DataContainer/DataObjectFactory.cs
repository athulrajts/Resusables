using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace KEI.Infrastructure
{
    /// <summary>
    /// Class used internally to create DataObjects
    /// </summary>
    public static class DataObjectFactory
    {
        /// <summary>
        /// Create mapping for <see cref="DataObject.Type"/> to it's implementation
        /// </summary>
        private static readonly Dictionary<string, Type> typeIdDataObjMapping = new Dictionary<string, Type>
        {
            { "bool" , typeof(BoolDataObject) },
            { "byte" , typeof(ByteDataObject) },
            { "int" , typeof(IntDataObject) },
            { "float" , typeof(FloatDataObject) },
            { "double" , typeof(DoubleDataObject) },
            { "string", typeof(StringDataObject) },
            { "enum" , typeof(EnumDataObject) },
            { "dc" , typeof(ContainerDataObject) },
            { "char", typeof(CharDataObject)},
            { "color", typeof(ColorDataObject)},
            { "dcl", typeof(CollectionDataObject) },
            { "array-1", typeof(Array1DDataObject)},
            { "array-2", typeof(Array2DDataObject)},
            { "dt", typeof(DateTimeDataObject)},
            { "ts", typeof(TimeSpanDataObject)},
            { "pt", typeof(PointDataObject)},
            { "xml", typeof(XmlDataObject) },
            { "json", typeof(JsonDataObject) }
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
            { "bool" , typeof(BoolPropertyObject) },
            { "byte" , typeof(BytePropertyObject) },
            { "int" , typeof(IntPropertyObject) },
            { "float" , typeof(FloatPropertyObject) },
            { "double" , typeof(DoublePropertyObject) },
            { "string", typeof(StringPropertyObject) },
            { "enum" , typeof(EnumPropertyObject) },
            { "opt" , typeof(SelectablePropertyObject) },
            { "dc" , typeof(ContainerPropertyObject) },
            { "char", typeof(CharPropertyObject)},
            { "color", typeof(ColorPropertyObject)},
            { "dcl", typeof(CollectionPropertyObject)},
            { "file", typeof(FilePropertyObject)},
            { "folder", typeof(FolderPropertyObject)},
            { "array-1", typeof(Array1DPropertyObject) },
            { "array-2", typeof(Array2DPropertyObject) },
            { "dt", typeof(DateTimePropertyObject)},
            { "ts", typeof(TimeSpanPropertyObject)},
            { "pt", typeof(PointPropertyObject)},
            { "xml", typeof(XmlPropertyObject)},
            { "json", typeof(JsonPropertyObject)}
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
            { typeof(DateTime), typeof(DateTimeDataObject) },
            { typeof(TimeSpan), typeof(TimeSpanDataObject)},
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
                    ObservableCollection<IDataContainer> ie => new CollectionDataObject(name, ie),
                    Array a => GetArrayDataObject(name, a),
                    IList l => new CollectionDataObject(name, l),
                    _ => new ContainerDataObject(name, value),
                };
            }

            static DataObject GetArrayDataObject(string name, Array a)
            {
                if(a.GetType().GetElementType().IsPrimitive == false)
                {
                    throw new NotSupportedException("Array of non primitive types not supported");
                }

                return a.Rank switch
                {
                    1 => new Array1DDataObject(name, a),
                    2 => new Array2DDataObject(name, a),
                    _ => throw new NotSupportedException("Array of more than 2 dimensions not allowed")
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
                    ObservableCollection<IDataContainer> ie => new CollectionPropertyObject(name, ie),
                    IDataContainer d => new ContainerPropertyObject(name, d),
                    Array a => GetArrayPropertyObject(name, a),
                    IList l => new CollectionPropertyObject(name, l),
                    _ => new ContainerPropertyObject(name, value),
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
                    _ => throw new NotSupportedException("Array of more than 2 dimensions not allowed")
                };
            }
        }

    }

    public static class CustomUITypeEditorMapping
    {
        public static Attribute ExpandableAttribute { get; set; }

        public static Dictionary<Type, Type> editorMapping = new Dictionary<Type, Type>();
        public static Dictionary<Type, Type> converterMapping = new Dictionary<Type, Type>();

        /// <summary>
        /// Registers an editor for <see cref="DataObject"/> with <see cref="DataObject.Type"/> equal to <paramref name="typeid"/>
        /// this type will be use as parameter for <see cref="EditorAttribute"/> which is used by 3rd party PropertyGrid implementations
        /// as well as WindowsForms property grid.
        /// </summary>
        /// <typeparam name="TEditor"></typeparam>
        /// <param name="typeid"></param>
        public static void RegisterEditor<TEditor>(string typeid)
        {
            var dataobj = DataObjectFactory.GetPropertyObject(typeid);

            if (dataobj is null)
            {
                return;
            }

            var type = dataobj.GetType();

            if (editorMapping.ContainsKey(type))
            {
                editorMapping[type] = typeof(TEditor);
            }
            else
            {
                editorMapping.Add(type, typeof(TEditor));
            }
        }

        /// <summary>
        /// Registers type converter for <see cref="DataObject"/> with <see cref="DataObject.Type"/> equal to <paramref name="typeid"/>
        /// this type will be use as parameter for <see cref="TypeConverterAttribute"/> which is used by WindowsForms PropertyGrid
        /// </summary>
        /// <typeparam name="TEditor"></typeparam>
        /// <param name="typeid"></param>
        public static void RegisterConverter<TConverter>(string typeid)
            where TConverter : TypeConverter
        {
            var dataobj = DataObjectFactory.GetPropertyObject(typeid);

            if (dataobj is null)
            {
                return;
            }

            var type = dataobj.GetType();

            if (converterMapping.ContainsKey(type))
            {
                converterMapping[type] = typeof(TConverter);
            }
            else
            {
                converterMapping.Add(type, typeof(TConverter));
            }
        }

        /// <summary>
        /// Called from <see cref="DataContainerBase.GetProperties(Attribute[])"/> to create <see cref="EditorAttribute"/>
        /// </summary>
        /// <param name="propertyObjectType"></param>
        /// <returns></returns>
        public static Type GetEditorType(Type propertyObjectType)
        {
            if (editorMapping.ContainsKey(propertyObjectType) == false)
            {
                return null;
            }

            return editorMapping[propertyObjectType];
        }

        /// <summary>
        /// Called from <see cref="DataContainerBase.GetProperties(Attribute[])"/> to create <see cref="TypeConverterAttribute"/>
        /// </summary>
        /// <param name="propertyObjectType"></param>
        /// <returns></returns>
        public static Type GetConverterType(Type propertyObjectType)
        {
            if (converterMapping.ContainsKey(propertyObjectType) == false)
            {
                return null;
            }

            return converterMapping[propertyObjectType];
        }

        public static Type PropertyGridEditor { get; set; }
        public static Type CollectionEditor { get; set; }

    }
}
