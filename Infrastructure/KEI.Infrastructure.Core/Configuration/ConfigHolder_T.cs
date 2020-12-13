using System.IO;
using System.Collections;
using System.Collections.Generic;
using Prism.Mvvm;
using KEI.Infrastructure.Logging;
using System.Xml;

namespace KEI.Infrastructure.Configuration
{
    public abstract class ConfigHolder<T> : BindableBase, IConfigHolder<T>
        where T: class, new()
    {

        #region Constructor
        
        public ConfigHolder()
        {

            if (LoadConfig() == false)
            {
                // Backup if file already exist inorder to check why it got corrupt
                if (File.Exists(ConfigPath))
                {
                    var dir = Path.GetDirectoryName(ConfigPath);
                    var fileName = Path.GetFileNameWithoutExtension(ConfigPath);
                    var ext = Path.GetExtension(ConfigPath);

                    File.Copy(ConfigPath, Path.Combine(dir, $"{fileName}_error.{ext}"), true);
                }

                CreateDefaultConfig();
                StoreConfig();
            }
        }

        #endregion

        #region Properties

        public T Config { get; set; } = new T();

        public abstract string ConfigPath { get; }

        public abstract string ConfigName { get; }

        #endregion

        #region Public Functions
        public bool LoadConfig()
        {
            var intermediateConfig = XmlHelper.DeserializeFromFile<DataContainer>(ConfigPath);

            if (intermediateConfig == null || string.IsNullOrEmpty(ConfigPath))
            {
                Logger.Warning($"Unable to Load Config \"{ConfigName}\", Creating Default Config.");
                return false;
            }

            if (typeof(IList).IsAssignableFrom(typeof(T)))
            {
                Config = new T();

                if(Config is IList l)
                {
                    foreach (var item in intermediateConfig)
                    {
                        if(item.GetValue() is IDataContainer dc)
                        {
                            l.Add(dc.Morph());
                        }
                    }
                }
            }
            else
            {
                Config = intermediateConfig.Morph<T>();
            }

            return true;
        }

        public bool ResetConfig()
        {
            CreateDefaultConfig();

            return StoreConfig();
        }

        public bool StoreConfig() => StoreConfig(ConfigPath);

        public bool StoreConfig(string path)
        {
            if (typeof(IList).IsAssignableFrom(typeof(T)))
            {
                if(Config is IList listConfig)
                {
                    var dc = DataContainerBuilder.Create(ConfigName);
                    foreach (var item in DataContainerBuilder.CreateList(ConfigName, listConfig))
                    {
                        dc.Data(item.Name, item);
                    }

                    if (XmlHelper.SerializeToFile(dc.Build(), path) == false)
                    {
                        Logger.Error($"Unable to Store Config \"{ConfigName}\"");
                        return false;
                    }

                }
            }
            else
            {
                if (XmlHelper.SerializeToFile(DataContainerBuilder.CreateObject(ConfigName,Config), path) == false)
                {
                    Logger.Error($"Unable to Store Config \"{ConfigName}\"");
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Protected Functions
        protected virtual void CreateDefaultConfig()
        {
            return;
        }

        #endregion
    }
}
