using System.IO;
using System.Collections;
using System.Collections.Generic;
using Prism.Mvvm;

namespace KEI.Infrastructure.Configuration
{
    public abstract class ConfigHolder<T> : BindableBase, IConfigHolder<T> where T: class, new()
    {

        protected readonly IViewService _viewService;

        #region Constructor
        public ConfigHolder(IViewService viewService)
        {
            _viewService = viewService;

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
                _viewService.Warn($"Unable to Load Config \"{ConfigName}\", Creating Default Config.");
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

        public bool StoreConfig()
        {
            var fullPath = System.IO.Path.GetFullPath(ConfigPath);

            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                if (XmlHelper.Serialize((Config as IEnumerable<object>).ToListDataContainer(ConfigName), ConfigPath) == false)
                {
                    _viewService.Warn($"Unable to Store Config \"{ConfigName}\"");
                    return false;
                }
            }
            else
            {
                if (XmlHelper.Serialize(Config.ToDataContainer(ConfigName), ConfigPath) == false)
                {
                    _viewService.Warn($"Unable to Store Config \"{ConfigName}\"");
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
