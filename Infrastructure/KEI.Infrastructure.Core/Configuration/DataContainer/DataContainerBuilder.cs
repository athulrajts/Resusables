using System.Collections;
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
            if (config.ContainsData(name) || value is null)
            {
                return this;
            }

            config.Add(DataObjectFactory.GetDataObjectFor(name, value));

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

                objContainer.Data(prop.Name, prop.GetValue(value));
            }

            return objContainer.Build();
        }

        public static IDataContainer CreateList(string name, IList list)
        {
            if (list is null)
            {
                return null;
            }

            var listConfig = new DataContainerBuilder(name);
            
            listConfig.SetUnderlyingType(list.GetType());

            int count = 0;
            foreach (var obj in list)
            {
                listConfig.Data($"{obj.GetType().Name}[{count}]", obj);
                count++;
            }

            return listConfig.Build();
        }

        public static IDataContainer FromFile(string path) => DataContainer.FromFile(path);

        public static DataContainerBuilder Create(string name) => new DataContainerBuilder(name);
    }
}
