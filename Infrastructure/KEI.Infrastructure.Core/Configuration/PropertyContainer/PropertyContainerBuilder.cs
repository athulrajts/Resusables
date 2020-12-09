using System;
using System.Reflection;
using System.Collections;
using KEI.Infrastructure.Helpers;
using System.Collections.Generic;

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
        protected IPropertyContainer config;


        #endregion

        #region Constructor

        private PropertyContainerBuilder()
        {
            config = new PropertyContainer();
        }

        private PropertyContainerBuilder(string configName)
        {
            config = new PropertyContainer { Name = configName };
        }

        public static PropertyContainerBuilder Create(string name)
            => new PropertyContainerBuilder(name);

        public static PropertyContainerBuilder Create()
            => new PropertyContainerBuilder();

        #endregion

        public static IPropertyContainer FromFile(string path) => PropertyContainer.FromFile(path);


        public PropertyContainerBuilder Object(string name, object value, Action<PropertyObjectBuilder> propertyBuilder = null)
        {
            if (config.ContainsData(name) || value is null)
            {
                return this;
            }

            if (DataObjectFactory.GetPropertyObjectFor(name, value) is PropertyObject obj)
            {
                propertyBuilder?.Invoke(new PropertyObjectBuilder(obj));

                config.Add(obj);
            }

            return this;
        }

        public PropertyContainerBuilder File(string name, string value, Action<FilePropertyObjectBuilder> propertyBuilder = null)
        {
            if (config.ContainsData(name) || value is null)
            {
                return this;
            }


            var obj = new FolderPropertyObject(name, value);

            propertyBuilder?.Invoke(new FilePropertyObjectBuilder(obj));

            config.Add(obj);

            return this;
        }

        public PropertyContainerBuilder Color(string name, Color c, Action<PropertyObjectBuilder> propertyBuilder = null)
        {
            if (config.ContainsData(name))
            {
                return this;
            }

            var obj = new ColorPropertyObject(name, c);

            propertyBuilder?.Invoke(new PropertyObjectBuilder(obj));

            config.Add(obj);

            return this;
        }

        public PropertyContainerBuilder Array(string name, Array a, Action<PropertyObjectBuilder> propertyBuilder = null)
        {
            if (config.ContainsData(name))
            {
                return this;
            }

            if(a.GetType().GetElementType().IsPrimitive == false)
            {
                throw new NotSupportedException("Arrays of non primitive types not supported");
            }

            PropertyObject obj = a.Rank switch
            {
                1 => new Array1DPropertyObject(name, a),
                2 => new Array2DPropertyObject(name, a),
                _ => throw new NotSupportedException("Arrays of more than 2 dimensions not supported"),
            };

            propertyBuilder?.Invoke(new PropertyObjectBuilder(obj));

            config.Add(obj);

            return this;
        }

        public PropertyContainerBuilder Color(string name, string hex , Action<PropertyObjectBuilder> propertyBuilder = null)
        {
            if(Infrastructure.Color.Parse(hex) is Color c)
            {
                Color(name, c, propertyBuilder);
            }    

            return this;
        }

        public PropertyContainerBuilder Color(string name, byte R, byte G, byte B, Action<PropertyObjectBuilder> propertyBuilder = null)
        {
            return Color(name, new Color(R, G, B), propertyBuilder);
        }

        public PropertyContainerBuilder Number(string name, int value, Action<NumericPropertyObjectBuilder> propertyBuilder = null)
        {
            return Number(name, (object)value, propertyBuilder);
        }
        
        public PropertyContainerBuilder Number(string name, double value, Action<NumericPropertyObjectBuilder> propertyBuilder = null)
        {
            return Number(name, (object)value, propertyBuilder);
        }
        
        public PropertyContainerBuilder Number(string name, float value, Action<NumericPropertyObjectBuilder> propertyBuilder = null)
        {
            return Number(name, (object)value, propertyBuilder);
        }

        private PropertyContainerBuilder Number(string name, object value, Action<NumericPropertyObjectBuilder> propertyBuilder = null)
        {
            if (config.ContainsData(name) || value is null)
            {
                return this;
            }

            if (DataObjectFactory.GetPropertyObjectFor(name, value) is PropertyObject obj)
            {
                propertyBuilder?.Invoke(new NumericPropertyObjectBuilder(obj));

                config.Add(obj);
            }

            return this;
        }

        public PropertyContainerBuilder Folder(string name, string value)
        {
            if (config.ContainsData(name) || value is null)
            {
                return this;
            }

            var obj = new FolderPropertyObject(name, value);

            config.Add(obj);

            return this;
        }


        internal PropertyContainerBuilder Property(PropertyInfo pi, object obj)
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

        public static IEnumerable<IPropertyContainer> CreateList(string name, IList list)
        {
            if (list is null)
            {
                return null;
            }

            var dataObj = new ContainerCollectionPropertyObject(name, list);

            return dataObj.Value as IEnumerable<IPropertyContainer>;
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
