using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KEI.Infrastructure
{
    public static class PropertyGridHelper
    {
        public static Attribute ExpandableAttribute { get; set; }

        public static Dictionary<string, Type> editorMapping = new Dictionary<string, Type>();
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
            if (editorMapping.ContainsKey(typeid))
            {
                editorMapping[typeid] = typeof(TEditor);
            }
            else
            {
                editorMapping.Add(typeid, typeof(TEditor));
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
        public static Type GetEditorType(string typeid)
        {
            if (editorMapping.ContainsKey(typeid) == false)
            {
                return null;
            }

            return editorMapping[typeid];
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
