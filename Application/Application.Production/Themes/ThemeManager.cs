using Prism.Mvvm;
using KEI.Infrastructure;
using Application.Core;

namespace Application.Production.Themes
{
    public class ThemeManager : BindableBase
    {
        private Theme theme;
        public Theme Theme
        {
            get { return theme; }
            set { SetProperty(ref theme, value, () => UpdateResources(Theme)); }
        }

        public ThemeManager(GeneralPreferences preferences)
        {
            preferences.SetBinding("Theme", () => Theme, BindingMode.OneWay);
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
