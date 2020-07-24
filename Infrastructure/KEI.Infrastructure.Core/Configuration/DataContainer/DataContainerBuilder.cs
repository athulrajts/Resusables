using KEI.Infrastructure.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TypeInfo = KEI.Infrastructure.Types.TypeInfo;

namespace KEI.Infrastructure.Configuration
{
    public class DataContainerBuilder
    {
        DataContainer config = new DataContainer();
        public DataContainerBuilder(string name)
        {
            config = new DataContainer() { Name = name };
        }
        public DataContainer Build() => config;

        private void SetUnderlyingType(TypeInfo t) => config.UnderlyingType = t;

        internal DataContainerBuilder WithProperty(string name, object value)
        {
            if (config.ContainsProperty(name) || value is null)
                return this;

            config.Data.Add(new DataObject
            {
                Name = name,
                ValueString = value.ToString(),
                Value = value,
            });

            return this;
        }

        public DataContainerBuilder WithEnum(string name, Enum enumType)
        {
            if (config.ContainsProperty(name))
                return this;

            config.Data.Add(new DataObject
            {
                Name = name,
                Value = new Selector(enumType)
            });

            return this;
        }

        public DataContainerBuilder WithObject<T>(string name, T value)
            where T : class
        {
            if (config.ContainsProperty(name) || value is null)
                return this;

            var objectConfig = new DataContainerBuilder(name);

            objectConfig.SetUnderlyingType(new TypeInfo(value.GetType()));

            var props = value == null ? typeof(T).GetProperties() : value.GetType().GetProperties();

            foreach (var prop in props)
            {
                if (!prop.CanWrite)
                    continue;

                if (prop.PropertyType.IsPrimitiveType())
                {
                    if (prop.PropertyType.IsEnum)
                    {
                        objectConfig.WithEnum(prop.Name, (Enum)prop.GetValue(value));
                    }
                    else
                    {
                        objectConfig.WithProperty(prop.Name, prop.GetValue(value));
                    }
                }

                else if (prop.PropertyType.IsList())
                {
                    var obj = prop.GetValue(value) as IList;

                    if (prop.PropertyType.IsGenericType)
                    {
                        var listConfig = new DataContainerBuilder(prop.Name);
                        listConfig.config.UnderlyingType = new TypeInfo(prop.PropertyType);
                        for (int i = 0; i < obj.Count; i++)
                        {
                            listConfig.WithObject($"{obj[i].GetType().Name}[{i}]", obj[i]);
                        }
                        objectConfig.WithProperty(prop.Name, listConfig.Build());
                    }
                }
            }

            WithProperty(name, objectConfig.Build());

            return this;
        }

        public static IDataContainer CreateObject<T>(string name, T value)
            where T : class
        {
            if (value is null)
                return null;

            var objectCfg = new DataContainerBuilder(name);

            objectCfg.SetUnderlyingType(new TypeInfo(value.GetType()));

            var props = value == null ? typeof(T).GetProperties() : value.GetType().GetProperties();

            foreach (var prop in props)
            {
                if (!prop.CanWrite)
                    continue;

                if (prop.PropertyType.IsPrimitiveType())
                {
                    if (prop.PropertyType.IsEnum)
                    {
                        objectCfg.WithEnum(prop.Name,(Enum)prop.GetValue(value));
                    }
                    else
                    {
                        objectCfg.WithProperty(prop.Name, prop.GetValue(value));
                    }
                }
                else if (prop.PropertyType.IsList())
                {
                    objectCfg.WithProperty(prop.Name, CreateList(prop.Name, prop.GetValue(value) as IEnumerable<object>));
                }
                else
                {
                    objectCfg.WithObject(prop.Name, prop.GetValue(value));
                }

            }

            return objectCfg.Build();
        }

        public static IDataContainer CreateList(string name, IEnumerable<object> list)
        {
            if (list is null)
                return null;

            var listConfig = new DataContainerBuilder(name);
            listConfig.SetUnderlyingType(new TypeInfo(list.GetType()));
            for (int i = 0; i < list.Count(); i++)
            {
                listConfig.WithObject($"{list.ElementAt(i).GetType().Name}[{i}]", list.ElementAt(i));
            }
            return listConfig.Build();
        }
    }
}
