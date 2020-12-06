using Application.Core;
using Application.Production.Views;
using KEI.Infrastructure;
using KEI.Infrastructure.Prism;
using KEI.Infrastructure.Screen;
using KEI.UI.Wpf.Hotkey;
using System;
using System.Collections.Generic;
using System.Net;

namespace Application.Production.ViewModels
{
    public class TestClass
    {
        public int[] MyProperty { get; set; } = new[] { 1, 1, 2, 3, 5, 8, 13 };
    }

    public class Point
    {
        public int X { get; set; } = 22;
        public int Y { get; set; } = 22;
    }

    [Screen(DisplayName = "Demos 3",
     Icon = Icon.Property16x,
     ScreenName = nameof(DemoScreen3))]
    public class DemoScreen3ViewModel : BaseScreenViewModel
    {
        public IPropertyContainer SamplePropertyContainer { get; set; }

        public TestClass TestObject { get; set; } = new TestClass();

        public DemoScreen3ViewModel(IHotkeyService hotkeyService) : base(hotkeyService)
        {
            var list = new List<Point> { new(), new(), new() };

            IPropertyContainer inner = PropertyContainerBuilder.Create("Inner Container")
                .Property("InnerInt", 23)
                .Property("inner Enum", UriHostNameType.Dns)
                .Build();

            //IPropertyContainer inner2 = PropertyContainerBuilder.CreateList("InnerList", list);

            SamplePropertyContainer = PropertyContainerBuilder.Create("Sample Confing")
                .Property("IntegerProperty", 22).SetDescription("Integer Property").SetDisplayName("The Integer")
                .Property("FloalProperty", 3.14).SetDescription("Float Property")
                .Property("DoubleProperty", 3.14142).SetDescription("Double Property")
                .Property("BoolProperty", false).SetDescription("Bool Property")
                .Property("StringProperty", "Hello World").SetDescription("String Property").SetCategory("Strings")
                .FileProperty("FileProperty", @".\Configs\DefaultRecipe.rcp", new Tuple<string, string>("Recipe", "*.rcp")).SetDescription("File Property").SetCategory("Strings")
                .FolderProperty("FolderProperty", @".\Configs").SetDescription("Folder Property").SetCategory("Strings")
                .Property("EnumProperty", ApplicationMode.Production).SetDescription("Enum Property")
                .Property("ColorProperty", new Color(0,0,0)).SetDescription("Color Property")
                .Property("PropertyContainerProperty", inner).SetCategory("Complex")
                .Property("ListProperty", list).SetCategory("Complex")
                .Build();

            var s = XmlHelper.SerializeToString(SamplePropertyContainer);
            var d = XmlHelper.DeserializeFromString<PropertyContainer>(s);
        }

    }
}
