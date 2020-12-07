using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TypeInfo = KEI.Infrastructure.Types.TypeInfo;

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

        private void SetUnderlyingType(TypeInfo t) => config.UnderlyingType = t;

        public DataContainerBuilder Data(string name, object value)
        {
            if (config.ContainsData(name) || value is null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return this;
            }

            config.Add(DataObjectFactory.GetDataObjectFor(name, value));

            return this;
        }

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

        public static IEnumerable<IDataContainer> CreateList(string name, IList list)
        {
            if (list is null)
            {
                return null;
            }

            var dataObject = new ContainerCollectionDataObject(name, list);

            return dataObject.Value;
        }

        public static IDataContainer FromFile(string path) => DataContainer.FromFile(path);

        public static DataContainerBuilder Create(string name) => new DataContainerBuilder(name);
    }
}
