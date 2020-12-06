using System;
using System.Collections;
using System.Collections.Generic;
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
            { "dcl", typeof(ContainerCollectionDataObject) }
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
            { typeof(Color), typeof(ColorDataObject) }
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
            { "file", typeof(FilePropertyObject)},
            { "folder", typeof(FolderPropertyObject)},
            { "dcl", typeof(ContainerCollectionPropertyObject) }
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
            { typeof(Color), typeof(ColorPropertyObject)}
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
            var instance = (PropertyObject)FormatterServices.GetUninitializedObject(typeof(T));

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
                : null;
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
                : null;
        }

        /// <summary>
        /// Gets an initialized <see cref="DataObject"/> implementation for given value and name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DataObject GetDataObjectFor(string name, object value, params object[] args)
        {
            if(value is null)
            {
                throw new NullReferenceException();
            }

            Type valueType = value.GetType();

            if(typeDataObjMapping.ContainsKey(valueType))
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
                return value switch
                {
                    Enum e => new EnumDataObject(name, e),
                    IDataContainer d => new ContainerDataObject(name, d),
                    IList l => new ContainerCollectionDataObject(name, l),
                    DataObject data => data,
                    _ => new ContainerDataObject(name, value),
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
                return value switch
                {
                    Enum e => new EnumPropertyObject(name, e),
                    IDataContainer d => new ContainerPropertyObject(name, d),
                    IList l => new ContainerCollectionPropertyObject(name, l),
                    PropertyObject data => data,
                    _ => new ContainerPropertyObject(name, value),
                };
            }
        }

    }

    public static class CustomUITypeEditorMapping
    {
        public static Dictionary<Type, Type> editorMapping = new Dictionary<Type, Type>();
        public static Dictionary<Type, Type> converterMapping = new Dictionary<Type, Type>();

        public static void RegisterEditor<TEditor>(string typeid)
        {
            var dataobj = DataObjectFactory.GetPropertyObject(typeid);

            if(dataobj is null)
            {
                return;
            }

            var type = dataobj.GetType();

            if(editorMapping.ContainsKey(type))
            {
                editorMapping[type] = typeof(TEditor);
            }
            else
            {
                editorMapping.Add(type, typeof(TEditor));
            }
        }

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

        public static Type GetEditorType(Type propertyObjectType)
        {
            if(editorMapping.ContainsKey(propertyObjectType) == false)
            {
                return null;
            }

            return editorMapping[propertyObjectType];
        }

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
