using Application.Core;
using Application.UI;
using KEI.Infrastructure.Configuration;
using System;
using System.ComponentModel;
using System.Windows;

namespace ApplicationShell.Themes
{
    public class ThemeManager : IWeakEventListener
    {
        IDataContainer generalPreferences;
        public ThemeManager()
        {
            generalPreferences = CommonServiceLocator.ServiceLocator.Current.GetInstance<GeneralPreferences>().Config;

            PropertyChangedEventManager.AddListener(generalPreferences, this, "Theme");

            UpdateTheme();
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(PropertyChangedEventManager))
            {
                var eArgs = e as PropertyChangedEventArgs;

                if (eArgs.PropertyName == "Theme")
                {
                    UpdateTheme();
                }

                return true;
            }
            return false;
        }

        private void UpdateTheme()
        {
            Theme newTheme = Theme.Light;
            generalPreferences.Get("Theme", ref newTheme);

            UpdateResources(newTheme);
        }

        private void SetResource(string key, object value) => System.Windows.Application.Current.Resources[key] = value;
        private object GetResource(string key) => System.Windows.Application.Current.Resources[key];

        private void UpdateResources(Theme theme)
        {
            SetResource("Theme.Background", GetResource($"{theme}.Background"));
            SetResource("Theme.Accent", GetResource($"{theme}.Accent"));
            SetResource("Theme.Button", GetResource($"{theme}.Button"));
            SetResource("Theme.ButtonForeground", GetResource($"{theme}.ButtonForeground"));
            SetResource("Theme.ButtonSelected", GetResource($"{theme}.ButtonSelected"));
            SetResource("Theme.ButtonSelectedForeground", GetResource($"{theme}.ButtonSelectedForeground"));
            SetResource("Theme.AccentForeground", GetResource($"{theme}.AccentForeground"));
            SetResource("Theme.AccentBackgroundGradient", GetResource($"{theme}.AccentBackgroundGradient"));
            SetResource("Theme.ButtonBorder", GetResource($"{theme}.ButtonBorder"));
            SetResource("Theme.AccentBorder", GetResource($"{theme}.AccentBorder"));
        }
    }
}
