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
        protected readonly IViewService _viewService;
        protected readonly ILogger _logger;
        protected readonly IEventAggregator _eventAggregator;

        #region Constructor
        public ConfigHolder(IEssentialServices essentialServices)
        {
            _viewService = essentialServices.ViewService;
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
                CheckIntegrity();
            }

           _eventAggregator.GetEvent<ConfigLoadedEvent>().Publish(Config as PropertyContainer);
        }

        private void CheckIntegrity()
        {
            var shape = DefineConfigShape().Build() as PropertyContainer;
            List<Action> actions = new List<Action>();

            AddNewProperties(Config as PropertyContainer, shape, ref actions);

            RemoveObsoleteProperties(Config as PropertyContainer, shape, ref actions);

            // Store updated config if changes were made.
            if(actions.Count > 0)
            {
                actions.ForEach(action => action());
                Config.Store();
            }

        }
        private void AddNewProperties(PropertyContainer workingCopy, PropertyContainer workingBase, ref List<Action> addActions)
        {
            foreach (PropertyObject prop in workingBase)
            {
                if (workingCopy.ContainsProperty(prop.Name) == false)
                {
                    addActions.Add(() => workingCopy.AddProperty(prop));

                    _logger.Info($"Added new property {prop.Name} = {prop.ValueString}");
                }
                else if (prop.Value is PropertyContainer p)
                {
                    AddNewProperties(workingCopy.Get<PropertyContainer>(prop.Name), p, ref addActions);
                }

            }
        }

        private void RemoveObsoleteProperties(PropertyContainer workingCopy, PropertyContainer workingBase, ref List<Action> removeActions)
        {
            foreach (PropertyObject prop in workingCopy)
            {
                if (workingBase.ContainsProperty(prop.Name) == false)
                {
                    removeActions.Add(() => workingCopy.RemoveProperty(prop));

                    _logger.Info($"Removed property {prop.Name} = {prop.ValueString}");
                }
                else if (prop.Value is PropertyContainer p)
                {
                    RemoveObsoleteProperties(workingCopy.Get<PropertyContainer>(prop.Name), p, ref removeActions);
                }
            }
        }

        public void RemoveBinding<T>(string propertyKey, Expression<Func<T>> expression) => Config.RemoveBinding(propertyKey, expression);
        public void SetBinding<T>(string propertyKey, Expression<Func<T>> expression, BindingMode mode = BindingMode.TwoWay) => Config.SetBinding(propertyKey, expression);

        #endregion

        #region IConfigHolder<T> Members
        public IPropertyContainer Config { get; set; }

        public abstract string ConfigPath { get; }

        public abstract string ConfigName { get; }

        public bool LoadConfig()
        {
            Config = PropertyContainer.FromFile(ConfigPath);

            if (Config == null)
            {
                //LOG
                _viewService.Warn($"Unable to load config \"{ConfigPath}\", Creating new config with default values");
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
                //LOG
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
