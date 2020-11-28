using KEI.Infrastructure;
using KEI.Infrastructure.Utils;
using KEI.Infrastructure.Service;
using KEI.Infrastructure.Configuration;

namespace Application.Core
{
    public class GeneralPreferences : ConfigHolder
    {
        public override string ConfigPath => PathUtils.GetPath("Configs/general_preferences.xcfg");

        public override string ConfigName => "GENERAL_PREFERENCES";

        protected override PropertyContainerBuilder DefineConfigShape()
        {
            return PropertyContainerBuilder.Create(ConfigName, ConfigPath)
                .Property("ShowCommandPanelOnLeftSide", false)
                    .SetDescription("Indices whether commands of each screen is shown on the left side")
                .Property("Theme", Theme.Dark)
                    .SetDescription("Current Application Theme")
                    .SetBrowsePermission(BrowseOptions.NonBrowsable);
        }

        public GeneralPreferences(IEssentialServices essentialServices) : base(essentialServices)
        {
        }
    }
}
