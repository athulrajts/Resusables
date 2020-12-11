using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace KEI.Infrastructure
{

    public class DataContainerBuilder
    {
        IDataContainer config;
        
        public DataContainerBuilder(string name)
        {
            config = new DataContainer() { Name = name };
        }

        public IDataContainer Build() => config;

        public void SetUnderlyingType(Type t) => config.UnderlyingType = t;

        /// <summary>
        /// Create a nested data container.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="containerBuilder"></param>
        /// <returns></returns>
        public DataContainerBuilder DataContainer(string name, Action<DataContainerBuilder> containerBuilder)
        {
            if (config.ContainsData(name))
            {
                return this;
            }

            var builder = Create(name);
            containerBuilder?.Invoke(builder);

            config.Add(new ContainerDataObject(name, builder.Build()));

            return this;
        }

        /// <summary>
        /// Generic method to add <see cref="DataObject"/> based on <see cref="Type"/> of <paramref name="value"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DataContainerBuilder Data(string name, object value)
        {
            if (config.ContainsData(name) || value is null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return this;
            }

            config.Add(DataObjectFactory.GetDataObjectFor(name, value));

            return this;
        }

        /// <summary>
        /// Internal method to create <see cref="IDataContainer"/> from CLR Objects
        /// </summary>
        /// <param name="pi"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private DataContainerBuilder Data(PropertyInfo pi, object obj)
        {
            var value = pi.GetValue(obj);

            if(value is null || string.IsNullOrEmpty(value.ToString()))
            {
                return this;
            }

            config.Add(DataObjectFactory.GetDataObjectFor(pi.Name, value));

            return this;
        }

        /// <summary>
        /// Create <see cref="IDataContainer"/> from <paramref name="value"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IDataContainer CreateObject(string name, object value)
        {
            if (value is null)
            {
                return null;
            }

            var objContainer = new DataContainerBuilder(name);

            objContainer.SetUnderlyingType(value.GetType());

            var props = value.GetType().GetProperties();

            foreach (var prop in props)
            {
                if (prop.CanWrite == false)
                {
                    continue;
                }

                objContainer.Data(prop, value);
            }

            return objContainer.Build();
        }

        /// <summary>
        /// Create <see cref="IEnumerable{IDataContainer}"/> from <paramref name="list"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<IDataContainer> CreateList(string name, IList list)
        {
            if (list is null)
            {
                return null;
            }

            var dataObject = new ContainerCollectionDataObject(name, list);

            return dataObject.Value;
        }

        public static IDataContainer FromFile(string path) => Infrastructure.DataContainer.FromFile(path);

        public static DataContainerBuilder Create(string name) => new DataContainerBuilder(name);
    }
}
