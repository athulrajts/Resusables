using KEI.Infrastructure.Helpers;
using KEI.Infrastructure.Types;
using KEI.Infrastructure.Validation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KEI.Infrastructure.Configuration
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
        private PropertyContainer config = new PropertyContainer();

        /// <summary>
        /// Filename if the config built need to be saved
        /// </summary>
        private string filename = string.Empty;

        #endregion

        #region Constructor

        private PropertyContainerBuilder()
        {
            config = new PropertyContainer();
            filename = string.Empty;
        }

        private PropertyContainerBuilder(string configName, string fileName = "")
        {
            config = new PropertyContainer { Name = configName, FilePath = fileName };
            filename = fileName;
        }

        public static PropertyContainerBuilder Create(string name, string filename = "") => new PropertyContainerBuilder(name, filename);

        public static PropertyContainerBuilder Create() => new PropertyContainerBuilder();

        #endregion

        public static IPropertyContainer FromFile(string path) => PropertyContainer.FromFile(path);

        #region Builder Methods

        /// <summary>
        /// Appends an Enumeration Property, Sets <see cref="PropertyObject.Editor"/> as <see cref="EditorType.Enum"/>
        /// and Sets allowed Values as <see cref="Enum.GetNames(Type)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="enumType"></param>
        /// <param name="description"></param>
        /// <param name="browseOption"></param>
        /// <returns></returns>
        public PropertyContainerBuilder WithEnum(string name, Enum enumType, string description = null, BrowseOptions browseOption = BrowseOptions.Browsable)
        {
            if (config.ContainsProperty(name))
                return this;

            config.AddProperty(new PropertyObject
            {
                Name = name,
                Description = description,
                Value = new Selector(enumType),
                Editor = EditorType.Enum,
                BrowseOption = browseOption
            });

            return this;
        }

        /// <summary>
        /// Appends integer property
        /// </summary>
        /// <param name="name">name of <see cref="PropertyObject"/></param>
        /// <param name="value">value of <see cref="PropertyObject"/></param>
        /// <param name="description">description for property</param>
        /// <param name="validation">validations for property</param>
        /// <param name="browseOptions">browse option for property</param>
        /// <returns><see cref="PropertyContainerBuilder"/> instance</returns>
        public PropertyContainerBuilder WithProperty(string name, int value, string description = null, ValidatorGroup validation = null, BrowseOptions browseOptions = BrowseOptions.Browsable)
            => WithProperty(name, (object)value, description, validation, browseOptions);

        /// <summary>
        /// Appends boolean property
        /// </summary>
        /// <param name="name">name of <see cref="PropertyObject"/></param>
        /// <param name="value">value of <see cref="PropertyObject"/></param>
        /// <param name="description">description for property</param>
        /// <param name="validation">validations for property</param>
        /// <param name="browseOptions">browse option for property</param>
        /// <returns><see cref="PropertyContainerBuilder"/> instance</returns>
        public PropertyContainerBuilder WithProperty(string name, bool value, string description = null, ValidatorGroup validation = null, BrowseOptions browseOptions = BrowseOptions.Browsable)
            => WithProperty(name, (object)value, description, validation, browseOptions);

        /// <summary>
        /// Appends character property
        /// </summary>
        /// <param name="name">name of <see cref="PropertyObject"/></param>
        /// <param name="value">value of <see cref="PropertyObject"/></param>
        /// <param name="description">description for property</param>
        /// <param name="validation">validations for property</param>
        /// <param name="browseOptions">browse option for property</param>
        /// <returns><see cref="PropertyContainerBuilder"/> instance</returns>
        public PropertyContainerBuilder WithProperty(string name, char value, string description = null, ValidatorGroup validation = null, BrowseOptions browseOptions = BrowseOptions.Browsable)
            => WithProperty(name, (object)value, description, validation, browseOptions);

        /// <summary>
        /// Appends float property
        /// </summary>
        /// <param name="name">name of <see cref="PropertyObject"/></param>
        /// <param name="value">value of <see cref="PropertyObject"/></param>
        /// <param name="description">description for property</param>
        /// <param name="validation">validations for property</param>
        /// <param name="browseOptions">browse option for property</param>
        /// <returns><see cref="PropertyContainerBuilder"/> instance</returns>
        public PropertyContainerBuilder WithProperty(string name, float value, string description = null, ValidatorGroup validation = null, BrowseOptions browseOptions = BrowseOptions.Browsable)
            => WithProperty(name, (object)value, description, validation, browseOptions);

        /// <summary>
        /// Appends double property
        /// </summary>
        /// <param name="name">name of <see cref="PropertyObject"/></param>
        /// <param name="value">value of <see cref="PropertyObject"/></param>
        /// <param name="description">description for property</param>
        /// <param name="validation">validations for property</param>
        /// <param name="browseOptions">browse option for property</param>
        /// <returns><see cref="PropertyContainerBuilder"/> instance</returns>
        public PropertyContainerBuilder WithProperty(string name, double value, string description = null, ValidatorGroup validation = null, BrowseOptions browseOptions = BrowseOptions.Browsable)
            => WithProperty(name, (object)value, description, validation, browseOptions);

        /// <summary>
        /// Appends string property
        /// </summary>
        /// <param name="name">name of <see cref="PropertyObject"/></param>
        /// <param name="value">value of <see cref="PropertyObject"/></param>
        /// <param name="description">description for property</param>
        /// <param name="validation">validations for property</param>
        /// <param name="browseOptions">browse option for property</param>
        /// <returns><see cref="PropertyContainerBuilder"/> instance</returns>
        public PropertyContainerBuilder WithProperty(string name, string value, string description = null, ValidatorGroup validation = null, BrowseOptions browseOptions = BrowseOptions.Browsable)
            => WithProperty(name, (object)value, description, validation, browseOptions);


        /// <summary>
        /// Appends a property who's value is restricted to a set of values given
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">name of <see cref="PropertyObject"/></param>
        /// <param name="selectedValue">value of <see cref="PropertyObject"/></param>
        /// <param name="allowedValues">allowed values for property</param>
        /// <param name="description">description for property</param>
        /// <param name="browseOption">browse option for property</param>
        /// <returns></returns>
        public PropertyContainerBuilder WithSelector<T>(string name, T selectedValue, List<T> allowedValues, string description = null, BrowseOptions browseOption = BrowseOptions.Browsable)
        {
            if (config.ContainsProperty(name))
                return this;

            config.AddProperty(new PropertyObject
            {
                Name = name,
                Description = description,
                Value = Selector.Create<T>(selectedValue, allowedValues.ToArray()),
                Editor = EditorType.Enum,
                BrowseOption = browseOption
            });


            return this;
        }
        public PropertyContainerBuilder WithPropertyContainer(string name, IPropertyContainer value, string description = null, ValidatorGroup validation = null, BrowseOptions browseOptions = BrowseOptions.Browsable)
        {
            if (config.ContainsProperty(name))
                return this;

            config.AddProperty(new PropertyObject
            {
                Name = name,
                Description = description,
                Value = value,
                Editor = EditorType.Object,
                BrowseOption = browseOptions
            });

            return this;
        }



        /// <summary>
        /// Appends a string property to the <see cref="PropertyContainer"/> and
        /// Sets the <see cref="PropertyObject.Editor"/> as <see cref="EditorType.File"/>
        /// and add a default validation checking whether the file exists
        /// </summary>
        /// <param name="property">name of <see cref="PropertyObject"/></param>
        /// <param name="value">file path</param>
        /// <param name="description">description for property</param>
        /// <returns><see cref="PropertyContainerBuilder"/> instance</returns>
    public PropertyContainerBuilder WithFile(string property, string value, string description = null)
        {
            if (config.ContainsProperty(property))
                return this;

            config.AddProperty(new PropertyObject
            {
                Name = property,
                ValueString = value?.ToString(),
                Description = description,
                Validation = ValidationBuilder.Create().File().Validator,
                Value = value,
                Editor = EditorType.File
            });

            return this;
        }


        /// <summary>
        /// Appends a string property to the <see cref="PropertyContainer"/> and
        /// Sets the <see cref="PropertyObject.Editor"/> as <see cref="EditorType.Folder"/>
        /// and add a default validation checking whether the folder exists
        /// </summary>
        /// <param name="property">name of <see cref="PropertyObject"/></param>
        /// <param name="value">folder path</param>
        /// <param name="description">description for property</param>
        /// <returns><see cref="PropertyContainerBuilder"/> instance</returns>
        public PropertyContainerBuilder WithFolder(string property, string value, string description = null)
        {
            if (config.ContainsProperty(property))
                return this;

            config.AddProperty(new PropertyObject
            {
                Name = property,
                ValueString = value?.ToString(),
                Description = description,
                Validation = ValidationBuilder.Create().Directory().Validator,
                Value = value,
                Editor = EditorType.Folder
            });

            return this;
        }

        /// <summary>
        /// Appends <see cref="PropertyContainer"/> with Given Object as references and stores it
        /// under in a <see cref="PropertyObject"/> with the given key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">name of <see cref="PropertyObject"/></param>
        /// <param name="value">value to be used as reference</param>
        /// <returns><see cref="PropertyContainerBuilder"/> instance</returns>
        public PropertyContainerBuilder WithObject<T>(string name, T value)
           where T : class
        {
            if (config.ContainsProperty(name) || value is null )
                return this;

            var objectConfig = new PropertyContainerBuilder(name);

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
                        objectConfig.WithEnum(prop.Name,
                            (Enum)prop.GetValue(value),
                            prop.GetDescription(),
                            prop.GetBrowseOption());
                    }
                    else
                    {
                        objectConfig.WithProperty(prop.Name,
                            prop.PropertyType,
                            prop.GetValue(value),
                            prop.GetDescription(),
                            prop.GetValidators(),
                            prop.GetBrowseOption());
                    }
                }

                else if (prop.PropertyType.IsList())
                {
                    var obj = prop.GetValue(value) as IList;

                    if (prop.PropertyType.IsGenericType)
                    {
                        var listConfig = new PropertyContainerBuilder(prop.Name);
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

        /// <summary>
        /// Returns the config with data and configs specified by the builder
        /// </summary>
        /// <returns>DataContainer Object</returns>
        public IPropertyContainer Build() => config;

        #endregion

        #region Private/Internal Methods

        /// <summary>
        /// Sets the underlying type of the PropertyContainer being built
        /// </summary>
        /// <param name="t">TypeInfo object</param>
        internal void SetUnderlyingType(TypeInfo t) => config.UnderlyingType = t;

        /// <summary>
        /// Sets the underlying type of the PropertyContainer being built
        /// </summary>
        /// <param name="t">Type object</param>
        internal void SetUnderlyingType(Type t) => SetUnderlyingType(new TypeInfo(t));

        /// <summary>
        /// Adds <see cref="PropertyObject"/> to the config
        /// </summary>
        /// <param name="name">Data Name</param>
        /// <param name="value">Data Value</param>
        /// <param name="description">Data Description</param>
        /// <returns>DataContainer Builder Object</returns>
        internal PropertyContainerBuilder WithProperty(string name, object value, string description = null, ValidatorGroup rule = null, BrowseOptions browseOption = BrowseOptions.Browsable)
        {
            if (config.ContainsProperty(name) || value is null)
                return this;

            config.AddProperty(new PropertyObject
            {
                Name = name,
                ValueString = value.ToString(),
                Description = description,
                Validation = rule,
                Value = value,
                Editor = value.GetType().GetEditorType(),
                BrowseOption = browseOption
            });

            return this;
        }

        /// <summary>
        /// Method Used by ConfigBuilder internal to Create PropertyContainer from CLR types
        /// </summary>
        /// <param name="property">Name of the Property</param>
        /// <param name="type"><see cref="Type"/> of the Property</param>
        /// <param name="value">Value of the Property</param>
        /// <param name="description">Description for Property</param>
        /// <param name="rules">Validation Rules for Property</param>
        /// <param name="browseOption">BrowseOption for Property</param>
        /// <returns></returns>
        internal PropertyContainerBuilder WithProperty(string property, Type type, object value, string description, ValidatorGroup rules, BrowseOptions browseOption)
        {
            if (config.ContainsProperty(property))
                return this;

            config.AddProperty(new PropertyObject
            {
                Name = property,
                ValueString = value?.ToString(),
                Description = description,
                Validation = rules,
                Value = value,
                Editor = type.GetEditorType(),
                BrowseOption = browseOption
            });

            return this;
        }

        /// <summary>
        /// Create a <see cref="PropertyContainer"/> by using an <see cref="IEnumerable{T}"/> as reference
        /// </summary>
        /// <typeparam name="T">Specialization for <see cref="IEnumerable{T}"/></typeparam>
        /// <param name="name">Name of the result <see cref="PropertyContainer"/></param>
        /// <param name="list"><see cref="IEnumerable{T}"/> that is used as reference to build <see cref="PropertyContainer"/></param>
        /// <returns><see cref="PropertyContainer"/> built using <see cref="IEnumerable{T}"/> as reference</returns>
        public static IPropertyContainer CreateList<T>(string name, IEnumerable<T> list)
            where T : class
        {
            if (list is null)
                return null;

            var listConfig = new PropertyContainerBuilder(name);
            listConfig.SetUnderlyingType(new TypeInfo(list.GetType()));
            for (int i = 0; i < list.Count(); i++)
            {
                listConfig.WithObject($"{list.ElementAt(i).GetType().Name}[{i}]", list.ElementAt(i));
            }
            return listConfig.Build();
        }

        /// <summary>
        /// Create a <see cref="PropertyContainer"/> intance based on an <see cref="object"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">Name of the result <see cref="PropertyContainer"/></param>
        /// <param name="value"><see cref="object"/> that is used a reference to build <see cref="PropertyContainer"/></param>
        /// <returns><see cref="PropertyContainer"/> built using <see cref="object"/> as reference</returns>
        public static IPropertyContainer CreateObject<T>(string name, T value)
            where T : class
        {
            if (value is null)
                return null;

            var objectCfg = new PropertyContainerBuilder(name);

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
                        objectCfg.WithEnum(prop.Name,
                            (Enum)prop.GetValue(value),
                            prop.GetDescription(),
                            prop.GetBrowseOption());
                    }
                    else
                    {
                        objectCfg.WithProperty(prop.Name,
                            prop.GetValue(value),
                            prop.GetDescription(),
                            prop.GetValidators(),
                            prop.GetBrowseOption());
                    }
                }
                else if (prop.PropertyType.IsList())
                {
                    objectCfg.WithProperty(prop.Name,
                        CreateList(prop.Name, prop.GetValue(value) as IEnumerable<object>),
                        prop.GetDescription(),
                        prop.GetValidators(),
                        prop.GetBrowseOption());
                }
                else
                {
                    objectCfg.WithObject(prop.Name, prop.GetValue(value));
                }

            }

            return objectCfg.Build();
        }

        #endregion
    }
}
