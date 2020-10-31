using Application.Core;
using Application.Production.Views;
using KEI.Infrastructure.Configuration;
using KEI.Infrastructure.Prism;
using KEI.Infrastructure.Screen;
using KEI.UI.Wpf.Hotkey;

namespace Application.Production.ViewModels
{
    [Screen(DisplayName = "Demos 3",
     Icon = Icon.Property16x,
     ScreenName = nameof(DemoScreen3))]
    public class DemoScreen3ViewModel : BaseScreenViewModel
    {
        public IPropertyContainer SamplePropertyContainer { get; set; }

        public DemoScreen3ViewModel(IHotkeyService hotkeyService) : base(hotkeyService)
        {
            SamplePropertyContainer = PropertyContainerBuilder.Create("Sample Confing")
                .WithProperty("IntegerProperty", 22, "Integer Property")
                .WithProperty("FloalProperty", 3.14, "Float Property")
                .WithProperty("DoubleProperty", 3.14142, "Double Property")
                .WithProperty("BoolProperty", false, "Boolean Property")
                .WithProperty("StringProperty", "Hello World", "String Property")
                .WithFile("FileProperty", @".\Configs\DefaultRecipe.rcp", "File Path Property")
                .WithFolder("FolderProperty", @".\Configs", "Folder Path Property")
                .WithEnum("EnumProperty", ApplicationMode.Production, "Enum Property")
                .WithObject("Object Property", this)
                .Build();
        }
    }
}
