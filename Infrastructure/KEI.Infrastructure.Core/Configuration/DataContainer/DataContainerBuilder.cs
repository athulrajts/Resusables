using System;
using System.Reflection;

namespace KEI.Infrastructure
{

    public class DataContainerBuilder
    {
        protected readonly IDataContainer config;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        internal DataContainerBuilder(string name)
        {
            config = new DataContainer() { Name = name };
        }

        /// <summary>
        /// Return contructed <see cref="IDataContainer"/>
        /// </summary>
        /// <returns></returns>
        public IDataContainer Build() { return config; }

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
        /// Creates a DataObject for storing CLR objects based on <paramref name="format"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public DataContainerBuilder Data<T>(string name, T value, SerializationFormat format)
            where T : class, new()
        {
            if (config.ContainsData(name) || value is null)
            {
                return this;
            }

            DataObject obj = format switch
            {
                SerializationFormat.Container => DataObjectFactory.GetDataObjectFor(name, value),
                SerializationFormat.Json => new JsonPropertyObject(name, value),
                SerializationFormat.Xml => new XmlPropertyObject(name, value),
                _ => throw new NotImplementedException()
            };

            config.Add(obj);

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
        /// Read DataContainer from file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IDataContainer FromFile(string path) => Infrastructure.DataContainer.FromFile(path);

        /// <summary>
        /// returns <see cref="IDataContainer"/> instance built
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DataContainerBuilder Create(string name) => new DataContainerBuilder(name);
    }

    public enum SerializationFormat
    {
        Container,
        Json,
        Xml
    }
}
