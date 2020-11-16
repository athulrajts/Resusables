using System.IO;
using System.Collections;
using System.Collections.Generic;
using Prism.Mvvm;
using KEI.Infrastructure.Logging;

namespace KEI.Infrastructure.Configuration
{
    public abstract class ConfigHolder<T> : BindableBase, IConfigHolder<T>
        where T: class, new()
    {

        #region Constructor
        public ConfigHolder()
        {

            if (!LoadConfig())
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
            var intermediateConfig = XmlHelper.Deserialize<DataContainer>(ConfigPath);

            if (intermediateConfig == null || string.IsNullOrEmpty(ConfigPath))
            {
                Logger.Warn($"Unable to Load Config \"{ConfigName}\", Creating Default Config.");
                return false;
            }

            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                Config = (T)intermediateConfig.MorphList();
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
            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                if (XmlHelper.Serialize((Config as IEnumerable<object>).ToListDataContainer(ConfigName), path) == false)
                {
                    Logger.Error($"Unable to Store Config \"{ConfigName}\"");
                    return false;
                }
            }
            else
            {
                if (XmlHelper.Serialize(Config.ToDataContainer(ConfigName), path) == false)
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
