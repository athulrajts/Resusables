using KEI.Infrastructure.Service;
using KEI.Infrastructure.Configuration;
using KEI.Infrastructure.Utils;

namespace Application.Core
{
    public class GeneralPreferences : ConfigHolder
    {
        public override string ConfigPath => PathUtils.GetPath("Configs/general_preferences.xcfg");

        public override string ConfigName => "GENERAL_PREFERENCES";

        protected override PropertyContainerBuilder DefineConfigShape()
        {
            return PropertyContainerBuilder.Create(ConfigName, ConfigPath)
                .WithProperty("ShowCommandPanelOnLeftSide", false, "Indices whether commands of each screen is shown on the left side")
                .WithEnum("Theme", Theme.Dark, "Current Application Theme", BrowseOptions.NonEditable);
        }

        public GeneralPreferences(IEssentialServices essentialServices) : base(essentialServices)
        {
        }
    }
}
