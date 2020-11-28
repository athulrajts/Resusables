using System.Windows.Input;
using Prism.Commands;
using KEI.Infrastructure;
using KEI.UI.Wpf;
using Application.Core;
using Application.Core.Interfaces;

namespace Application.UI.AdvancedSetup.ViewModels
{
    public class AppearanceViewModel : BaseViewModel, IAdvancedSetup
    {
        private readonly IConfigManager _configManager;
        private readonly ISystemStatusManager _statusManager;
        private readonly IPropertyContainer _generalPreferences;

        Theme currentTheme;
        bool currentShowCommandsOnLeft;
        public AppearanceViewModel(IConfigManager configManager, ISystemStatusManager systemStatusManager)
        {
            _configManager = configManager;
            _generalPreferences = configManager.GetConfig("GENERAL_PREFERENCES");
            _statusManager = systemStatusManager;

            GetInitialValues();

            SelectedTheme = currentTheme;
            ShowCommandsOnLeft = currentShowCommandsOnLeft;

        }


        private void GetInitialValues()
        {
            _generalPreferences.GetValue("Theme", ref currentTheme);
            _generalPreferences.GetValue("ShowCommandPanelOnLeftSide", ref currentShowCommandsOnLeft);
        }
        
        private Theme selectedTheme;
        public Theme SelectedTheme
        {
            get => selectedTheme;
            set => SetProperty(ref selectedTheme, value, () => UpdateResources(selectedTheme));
        }

        private bool showCommandsOnLeft;
        public bool ShowCommandsOnLeft
        {
            get => showCommandsOnLeft;
            set => SetProperty(ref showCommandsOnLeft, value, OnShowCommandsOnLeftChanged);
        }

        private void OnShowCommandsOnLeftChanged()
        {
            if(showCommandsOnLeft)
            {
                CommandPosition = 0;
                SubViewPosition = 2;
            }
            else
            {
                CommandPosition = 2;
                SubViewPosition = 0;
            }
            RaisePropertyChanged(nameof(SubViewPosition));
            RaisePropertyChanged(nameof(CommandPosition));
        }

        public int CommandPosition { get; set; } = 2;
        public int SubViewPosition { get; set; } = 0;


        private DelegateCommand updateConfig;
        public ICommand UpdateConfig
        {
            get
            {
                if (updateConfig == null)
                {
                    updateConfig = new DelegateCommand(() =>
                    {
                        bool needSave = false;
                        if (currentTheme != selectedTheme)
                        {
                            needSave = true;
                            _generalPreferences.SetValue("Theme", selectedTheme);
                            currentTheme = selectedTheme;
                        }
                        if (currentShowCommandsOnLeft != showCommandsOnLeft)
                        {
                            needSave = true;
                            _generalPreferences.SetValue("ShowCommandPanelOnLeftSide", showCommandsOnLeft);
                            currentShowCommandsOnLeft = showCommandsOnLeft;
                        }

                        if (needSave)
                        {
                            _generalPreferences.Store();
                        }

                    });
                }
                return updateConfig;
            }
        }

        public string Name => "Appearance";

        public bool IsAvailable => _statusManager.ApplicationMode == ApplicationMode.Production;

        #region Theme Changing

        private void SetResource(string key, object value) => System.Windows.Application.Current.Resources[key] = value;
        private object GetResource(string key) => System.Windows.Application.Current.Resources[key];

        private void UpdateResources(Theme theme)
        {
            SetResource("ThemeViewer.Background", GetResource($"{theme}.Background"));
            SetResource("ThemeViewer.Accent", GetResource($"{theme}.Accent"));
            SetResource("ThemeViewer.Button", GetResource($"{theme}.Button"));
            SetResource("ThemeViewer.ButtonForeground", GetResource($"{theme}.ButtonForeground"));
            SetResource("ThemeViewer.ButtonSelected", GetResource($"{theme}.ButtonSelected"));
            SetResource("ThemeViewer.ButtonSelectedForeground", GetResource($"{theme}.ButtonSelectedForeground"));
            SetResource("ThemeViewer.AccentForeground", GetResource($"{theme}.AccentForeground"));
            SetResource("ThemeViewer.AccentBackgroundGradient", GetResource($"{theme}.AccentBackgroundGradient"));
            SetResource("ThemeViewer.ButtonBorder", GetResource($"{theme}.ButtonBorder"));
            SetResource("ThemeViewer.AccentBorder", GetResource($"{theme}.AccentBorder"));
        }

        #endregion


    }
}
