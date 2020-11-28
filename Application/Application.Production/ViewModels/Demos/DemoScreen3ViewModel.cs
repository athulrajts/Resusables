using Application.Core;
using Application.Production.Views;
using KEI.Infrastructure;
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
                .Property("IntegerProperty", 22).SetDescription("Integer Property")
                .Property("FloalProperty", 3.14).SetDescription("Float Property")
                .Property("DoubleProperty", 3.14142).SetDescription("Double Property")
                .Property("BoolProperty", false).SetDescription("Bool Property")
                .Property("StringProperty", "Hello World").SetDescription("String Property")
                .Property("FileProperty", @".\Configs\DefaultRecipe.rcp").SetDescription("File Property")
                .Property("FolderProperty", @".\Configs").SetDescription("Folder Property")
                .Property("EnumProperty", ApplicationMode.Production).SetDescription("Enum Property")
                .Build();
        }
    }
}
