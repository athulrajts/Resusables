using System.Linq;
using System.Collections.ObjectModel;
using Prism.Events;
using KEI.Infrastructure.Screen;
using KEI.Infrastructure.Configuration;
using Application.Production.Screen;
using KEI.Infrastructure.Utils;

namespace KEI.Infrastructure
{
    public class ScreenConfig : ConfigHolder<ObservableCollection<ScreenInfo>>
    {

        public ObservableCollection<ScreenInfo> InactiveScreens { get; set; } = new ObservableCollection<ScreenInfo>();

        public override string ConfigPath => PathUtils.GetPath("Configs/screens.xcfg");

        public override string ConfigName => @"ScreenConfig";

        public bool ContainsScreen(string screenName)
        {
            return Config.SingleOrDefault(x => x.ScreenName == screenName) is ScreenInfo;
        }

        protected override void CreateDefaultConfig()
        {
            Config = new ObservableCollection<ScreenInfo>
            {
                new ScreenInfo
                {
                    ScreenName = "DemoScreen",
                    DisplayName = @"Demos",
                    Icon = Icon.ShowAllCode16x,
                    ParentScreenName = string.Empty
                },
                new ScreenInfo
                {
                    ScreenName = "ConfigScreen",
                    DisplayName = @"Configs",
                    Icon = Icon.Settings16x,
                    ParentScreenName = string.Empty,
                    IsMandatory = true
                },
                new ScreenInfo
                {
                    ScreenName = "ScreenConfigEditor",
                    DisplayName = @"Manage Screens",
                    Icon = Icon.None16x,
                    ParentScreenName = "ConfigScreen",
                    IsMandatory = true
                }
            };
        }
    }

    public class ScreenConfigUpdatedEvent : PubSubEvent<ObservableCollection<ScreenInfo>> { }
}
