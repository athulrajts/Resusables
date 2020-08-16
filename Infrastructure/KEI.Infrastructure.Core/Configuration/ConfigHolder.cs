using System;
using System.IO;
using System.Linq.Expressions;
using System.Collections.Generic;
using Prism.Mvvm;
using Prism.Events;
using KEI.Infrastructure.Service;

namespace KEI.Infrastructure.Configuration
{
    public abstract class ConfigHolder : BindableBase, IConfigHolder<IPropertyContainer>
    {
        protected readonly ILogger _logger;
        protected readonly IEventAggregator _eventAggregator;

        #region Constructor
        public ConfigHolder(IEssentialServices essentialServices)
        {
            _logger = essentialServices.LogManager.GetLogger(GetType());
            _eventAggregator = essentialServices.EventAggregator;

            if(LoadConfig() == false)
            {
                // Backup if file already exist inorder to check why it got corrupt
                if (File.Exists(ConfigPath))
                {
                    var dir = Path.GetDirectoryName(ConfigPath);
                    var fileName = Path.GetFileNameWithoutExtension(ConfigPath);
                    var ext = Path.GetExtension(ConfigPath);

                    File.Copy(ConfigPath, Path.Combine(dir, $"{fileName}_error.{ext}"), true);
                }

                // Create and store default
                Config = DefineConfigShape().Build();
                Config.Store();
            }
            else
            {
                if(Config.Merge(DefineConfigShape().Build()) == true)
                {
                    Config.Store();
                }
            }

           _eventAggregator.GetEvent<ConfigLoadedEvent>().Publish(Config);
        }

        #endregion

        public void RemoveBinding<T>(string propertyKey, Expression<Func<T>> expression) => Config.RemoveBinding(propertyKey, expression);
        public void SetBinding<T>(string propertyKey, Expression<Func<T>> expression, BindingMode mode = BindingMode.TwoWay) => Config.SetBinding(propertyKey, expression, mode);

        #region IConfigHolder<T> Members
        
        public IPropertyContainer Config { get; set; }

        public abstract string ConfigPath { get; }

        public abstract string ConfigName { get; }

        public bool LoadConfig()
        {
            Config = PropertyContainer.FromFile(ConfigPath);

            if (Config == null)
            {
                _logger.Warn($"Unable to load config \"{ConfigPath}\", Creating new config with default values");
                return false;
            }

            return true;
        }

        public bool StoreConfig()
        {
            if (Config == null)
                return false;

            if (Config.Store(ConfigPath) == false)
            {
                _logger.Error($"Unable to store confing \"{ConfigPath}\"");
                return false;
            }

            return true;

        }

        public bool GetValue<T>(string key, ref T value) => Config.Get(key, ref value);
        public void SetValue<T>(string key, T value) => Config.Set(key, value);
        protected abstract PropertyContainerBuilder DefineConfigShape();

        #endregion
    }
}
