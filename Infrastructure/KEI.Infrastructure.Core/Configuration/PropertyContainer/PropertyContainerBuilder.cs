using System;
using System.Reflection;
using System.Collections;
using KEI.Infrastructure.Helpers;

namespace KEI.Infrastructure
{

    /// <summary>
    /// Class use to build <see cref="PropertyContainer"/> Objects
    /// </summary>
    public class PropertyContainerBuilder
    {
        #region Fields

        /// <summary>
        /// Object the builder uses
        /// </summary>
        private IPropertyContainer config;

        /// <summary>
        /// Filename if the config built need to be saved
        /// </summary>
        private string filename = string.Empty;

        PropertyObject lastCreatedObject;

        #endregion

        #region Constructor

        private PropertyContainerBuilder()
        {
            config = new PropertyContainer();
        }

        private PropertyContainerBuilder(string configName, string fileName = "")
        {
            config = new PropertyContainer { Name = configName, FilePath = fileName };
            filename = fileName;
        }

        public static PropertyContainerBuilder Create(string name, string filename = "")
            => new PropertyContainerBuilder(name, filename);

        public static PropertyContainerBuilder Create()
            => new PropertyContainerBuilder();

        #endregion

        public static IPropertyContainer FromFile(string path) => PropertyContainer.FromFile(path);


        public PropertyContainerBuilder Property(string name, object value)
        {
            if (config.ContainsData(name) || value is null)
            {
                return this;
            }

            if (DataObjectFactory.GetPropertyObjectFor(name, value) is PropertyObject obj)
            {
                config.Add(obj);

                lastCreatedObject = obj;
            }

            return this;
        }

        public PropertyContainerBuilder FileProperty(string name, string value, params Tuple<string,string>[] filters)
        {
            if (config.ContainsData(name) || value is null)
            {
                return this;
            }

            var obj = new FilePropertyObject(name, value, filters);

            config.Add(obj);

            lastCreatedObject = obj;

            return this;
        }

        public PropertyContainerBuilder FolderProperty(string name, string value)
        {
            if (config.ContainsData(name) || value is null)
            {
                return this;
            }

            var obj = new FolderPropertyObject(name, value);

            config.Add(obj);

            lastCreatedObject = obj;

            return this;
        }


        public PropertyContainerBuilder SetDescription(string description)
        {
            if (lastCreatedObject is not null)
            {
                lastCreatedObject.SetDescription(description);
            }

            return this;
        }

        public PropertyContainerBuilder SetBrowsePermission(BrowseOptions option)
        {
            if (lastCreatedObject is not null)
            {
                lastCreatedObject.SetBrowsePermission(option);
            }

            return this;
        }

        public PropertyContainerBuilder SetDisplayName(string name)
        {
            if (lastCreatedObject is not null)
            {
                lastCreatedObject.SetDisplayName(name);
            }

            return this;
        }

        public PropertyContainerBuilder SetCategory(string category)
        {
            if (lastCreatedObject is not null)
            {
                lastCreatedObject.SetCategory(category);
            }

            return this;
        }

        public PropertyContainerBuilder Property(PropertyInfo pi, object obj)
        {

            if (pi.GetValue(obj) is not null)
            {
                var option = pi.GetBrowseOption();
                var description = pi.GetDescription();
                config.Add(DataObjectFactory.GetPropertyObjectFor(pi.Name, pi.GetValue(obj))
                    .SetBrowsePermission(option)
                    .SetDescription(description));
            }

            return this;
        }

        public static IPropertyContainer CreateObject(string name, object value)
        {
            if (value is null)
            {
                return null;
            }

            var objContainer = new PropertyContainerBuilder(name);

            objContainer.SetUnderlyingType(value.GetType());

            var props = value.GetType().GetProperties();

            foreach (var prop in props)
            {
                if (prop.CanWrite == false)
                {
                    continue;
                }

                objContainer.Property(prop, value);
            }

            return objContainer.Build();
        }

        public static IPropertyContainer CreateList(string name, IList list)
        {
            if (list is null)
            {
                return null;
            }

            var listConfig = new PropertyContainerBuilder(name);

            listConfig.SetUnderlyingType(list.GetType());

            int count = 0;
            foreach (var obj in list)
            {
                listConfig.Property($"{obj.GetType().Name}[{count}]", obj);
                count++;
            }

            return listConfig.Build();
        }


        /// <summary>
        /// Returns the config with data and configs specified by the builder
        /// </summary>
        /// <returns>DataContainer Object</returns>
        public IPropertyContainer Build() => config;

        #region Private/Internal Methods

        /// <summary>
        /// Sets the underlying type of the PropertyContainer being built
        /// </summary>
        /// <param name="t">Type object</param>
        internal void SetUnderlyingType(Type t) => config.UnderlyingType = t;

        #endregion
    }
}
