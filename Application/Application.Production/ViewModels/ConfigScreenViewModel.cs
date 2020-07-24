using System;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommonServiceLocator;
using Prism.Commands;
using KEI.Infrastructure;
using KEI.Infrastructure.Prism;
using KEI.Infrastructure.Types;
using KEI.Infrastructure.Screen;
using KEI.Infrastructure.Validation;
using KEI.Infrastructure.Configuration;
using KEI.UI.Wpf;
using KEI.UI.Wpf.Hotkey;
using KEI.UI.Wpf.Controls.ObjectEditors;
using Application.Core;
using Application.Production.Views;
using Application.Production.Screen;

namespace Application.Production.ViewModels
{

    [Screen(DisplayName = "Configs",
            Icon = Icon.Settings16x,
            ScreenName = nameof(ConfigScreen))]
    public class ConfigScreenViewModel : BaseScreenViewModel<ConfigScreen>
    {
        #region BaseScreenViewModel Members
        public override Icon Icon { get; set; } = Icon.Settings16x;
        public override string DisplayName { get; set; } = @"Configs";

        #endregion

        #region Injected Members

        private readonly IConfigManager _configManager;
        private readonly IApplicationViewService _viewService;
        private readonly List<Type> _validationImplementations;

        #endregion

        #region Private Fields
        private List<string> propertiesChanged = new List<string>();
        private Dictionary<string, Dictionary<string, ConfigHistoryItem>> changedValues = new Dictionary<string, Dictionary<string, ConfigHistoryItem>>();
        #endregion

        #region Constructor
        public ConfigScreenViewModel(IConfigManager configManager, IApplicationViewService viewService,
            IHotkeyService hotkeyService) : base(hotkeyService)
        {
            _configManager = configManager;
            _viewService = viewService;

            Configs = _configManager.Configs;

            Commands.Add(new ScreenCommand
            {
                DisplayName = "Update",
                Command = UpdateConfigCommand,
                Icon = Icon.Refresh16x
            });

            SelectedConfig = _configManager.Configs.FirstOrDefault();

            _validationImplementations = ServiceLocator.Current.GetInstance<ImplementationsProvider>().GetImplementations(typeof(ValidationRule));
        }

        #endregion

        #region Properties

        private IPropertyContainer selectedConfig;
        public IPropertyContainer SelectedConfig
        {
            get => selectedConfig;
            set => SetProperty(ref selectedConfig, value, OnSelectedConfigChanged);
        }

        private IPropertyContainer selectedConfigClone;
        public IPropertyContainer SelectedConfigClone
        {
            get => selectedConfigClone;
            set => SetProperty(ref selectedConfigClone, value);
        }


        private ObservableCollection<IPropertyContainer> configs;
        public ObservableCollection<IPropertyContainer> Configs
        {
            get => configs;
            set => SetProperty(ref configs, value);
        }


        private PropertyObject selectedData;
        public PropertyObject SelectedData
        {
            get { return selectedData; }
            set { SetProperty(ref selectedData, value); }
        }

        public ObservableCollection<IPropertyContainer> CachedConfigs { get; set; } = new ObservableCollection<IPropertyContainer>();

        #endregion

        #region Update Command

        private DelegateCommand updateConfigCommand;
        public DelegateCommand UpdateConfigCommand =>
            updateConfigCommand ?? (updateConfigCommand = new DelegateCommand(ExecuteUpdateConfigCommand));
        private void ExecuteUpdateConfigCommand()
        {
            // Remove entries whos New and Old values are same
            var actualChanges = new Dictionary<string, Dictionary<string, ConfigHistoryItem>>();

            foreach (var key in changedValues.Keys)
            {
                var actual = changedValues[key].Where(x => x.Value.HasChanges == true).ToDictionary(t => t.Key, t => t.Value);
                if (actual.Count > 0)
                    actualChanges.Add(key, actual);
            }

            if (actualChanges.Count > 0)
            {

                if (_viewService.ShowUpdatedConfigs(actualChanges) == PromptResult.Yes)
                {
                    foreach (var cfg in changedValues)
                    {
                        foreach (var property in cfg.Value)
                        {
                            property.Value.Commit();
                        }
                    }
                }
                else
                {
                    foreach (var cfg in changedValues)
                    {
                        foreach (var property in cfg.Value)
                        {
                            property.Value.Revert();
                        }
                    }
                }

            }
            else
            {
                if (_viewService.Prompt("Are you sure you want to update settings ? ", KEI.Infrastructure.PromptOptions.YesNo) == KEI.Infrastructure.PromptResult.Yes)
                {
                    foreach (var config in Configs)
                    {
                        config.Store();
                    }
                }
            }

            changedValues.Clear();
        }

        protected override void ProcessPropertyChanged(object sender, string property)
        {
            if (property == nameof(PropertyObject.ValueString) || property == nameof(PropertyObject.Validation))
                return;

            if (sender is IPropertyContainer dc)
            {

                if (!changedValues.ContainsKey(dc.Name))
                    changedValues.Add(dc.Name, new Dictionary<string, ConfigHistoryItem>());

                if (!changedValues[dc.Name].ContainsKey(property))
                {
                    object currentValue = null;

                    var origConfig = Configs.FirstOrDefault(x => x.Name == dc.Name);

                    origConfig.Get(property, ref currentValue);

                    changedValues[dc.Name].Add(property, new ConfigHistoryItem(origConfig, dc, property));
                }
                else
                {
                    object newValue = null;
                    dc.Get(property, ref newValue);

                    if (newValue is Selector s)
                        newValue = s.SelectedItem;

                    changedValues[dc.Name][property].NewValue = newValue;
                }

            }
        }

        #endregion

        #region Launch Object Editor

        private DelegateCommand<object> launchEditorCommand;
        public DelegateCommand<object> LaunchEditorCommand =>
            launchEditorCommand ?? (launchEditorCommand = new DelegateCommand<object>(ExecuteLaunchEditorCommand));

        void ExecuteLaunchEditorCommand(object obj)
        {
            new ObjectEditorWindow(obj).ShowDialog();
            selectedData.ForceValueChanged();
        }

        #endregion

        #region Edit Validations

        private DelegateCommand editValidationGroup;
        public DelegateCommand EditValidationGroup =>
            editValidationGroup ?? (editValidationGroup = new DelegateCommand(ExecuteEditValidationGroup));

        void ExecuteEditValidationGroup()
        {
            if(selectedData.Validation is null)
            {
                selectedData.Validation = ValidationBuilder.Create();
            }

            new ObjectEditorWindow(selectedData.Validation.Rules, _validationImplementations).ShowDialog();

            if (selectedData.Validation.Rules.Count == 0)
            {
                selectedData.Validation = null;
            }
            else
            {
                selectedData.ForceValueChanged();
            }
        }

        #endregion

        #region Private Functions

        private void OnSelectedConfigChanged()
        {
            if (SelectedConfigClone != null)
            {
                RemovePropertyObserver(SelectedConfig, string.Empty);
            }

            if (CachedConfigs.SingleOrDefault(x => x.Name == SelectedConfig.Name) is IPropertyContainer cfg)
            {
                SelectedConfigClone = cfg;
            }
            else
            {
                SelectedConfigClone = (IPropertyContainer)SelectedConfig.Clone();
                CachedConfigs.Add(SelectedConfigClone);
            }


            if (SelectedConfigClone != null)
            {
                AddPropertyObserver(SelectedConfigClone, string.Empty);
            }
        }

        #endregion

        #region Hotkeys

        public override void RegisterHotkeys()
        {
            // Save Config
            _hotkeyService.AddReadonlyGesture(GestureCache.GetGesture($"{ScreenName}.Save",
                UpdateConfigCommand,
                null,
                Key.S,
                ModifierKeys.Control));
        }

        public override void UnregisterHotkeys()
        {
            // Save Config
            _hotkeyService.RemoveGesture($"{_assemblyName}.{ScreenName}.Save");
        }

        #endregion
    }
}
