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
            return PropertyContainerBuilder.Create(ConfigName)
                .Property("ShowCommandPanelOnLeftSide", false)
                .Property("Theme", Theme.Dark);
        }

        public GeneralPreferences(IEssentialServices essentialServices) : base(essentialServices)
        {
        }
    }
}
