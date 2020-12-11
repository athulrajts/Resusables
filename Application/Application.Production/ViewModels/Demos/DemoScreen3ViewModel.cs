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
    public enum Shape
    {
        Rectangle,
        Ellipse
    }

    [Screen(DisplayName = "Demos 3",
     Icon = Icon.Property16x,
     ScreenName = nameof(DemoScreen3))]
    public class DemoScreen3ViewModel : BaseScreenViewModel
    {
        public IPropertyContainer SamplePropertyContainer { get; set; }

        public DemoScreen3ViewModel(IHotkeyService hotkeyService) : base(hotkeyService)
        {
            IPropertyContainer inner = PropertyContainerBuilder.Create("Inner Container")
                .Property("InnerInt", 23)
                .Property("inner Enum", UriHostNameType.Dns)
                .Build();

            //IPropertyContainer inner2 = PropertyContainerBuilder.CreateList("InnerList", list);

            //SamplePropertyContainer = PropertyContainerBuilder.Create("Sample Confing")
            //    .Property("IntegerProperty", 22).SetDescription("Integer Property").SetDisplayName("The Integer")
            //    .Property("FloalProperty", 3.14).SetDescription("Float Property")
            //    .Property("DoubleProperty", 3.14142).SetDescription("Double Property")
            //    .Property("BoolProperty", false).SetDescription("Bool Property")
            //    .Property("StringProperty", "Hello World").SetDescription("String Property").SetCategory("Strings")
            //    .FileProperty("FileProperty", @".\Configs\DefaultRecipe.rcp", new Tuple<string, string>("Recipe", "*.rcp")).SetDescription("File Property").SetCategory("Strings")
            //    .FolderProperty("FolderProperty", @".\Configs").SetDescription("Folder Property").SetCategory("Strings")
            //    .Property("EnumProperty", ApplicationMode.Production).SetDescription("Enum Property")
            //    .Property("ColorProperty", new Color(0,0,0)).SetDescription("Color Property")
            //    .Property("PropertyContainerProperty", inner).SetCategory("Complex")
            //    .Property("ListProperty", list).SetCategory("Complex")
            //    .Build();
            Array a = new int[,] { { 11, 21 ,31 ,41 ,51}, { 61, 71, 81, 91, 10 }, { 11, 12, 13, 14, 15 }, { 16, 17, 18, 19, 20 } };

            SamplePropertyContainer = PropertyContainerBuilder.Create("Sample Confing")
                .Color("Fill", "#FFFFFF", p => p
                    .SetCategory("Visualization")
                    .SetDescription("Color of shape"))
                .Color("Stroke", "#000000", p => p
                    .SetCategory("Visualization")
                    .SetDescription("Border color of shape"))
                .Number("StrokeThickness", 1.0, p => p
                    .SetCategory("Visualization")
                    .SetDescription("Border thickness of shape"))
                .Property("Shape", Shape.Rectangle, p => p
                    .SetCategory("Definition")
                    .SetDescription("Ellipse/Rectangle"))
                .Number("Height", 200.0, p => p
                    .SetCategory("Definition")
                    .SetDescription("Height of shape"))
                .Number("Width", 200.0, p => p
                    .SetCategory("Definition")
                    .SetDescription("Widht of shape"))
                .Number("X", 50.0, p => p
                    .SetCategory("Definition")
                    .SetDescription("X-Coordinate of top left point"))
                .Number("Y", 50.0, p => p
                    .SetCategory("Definition")
                    .SetDescription("Y-Coordinate of top left point"))
                .Array("Array", a)
                .Time("Time", TimeSpan.FromSeconds(69))
                .DateTime("DateTime", DateTime.Now)
                .Property("TrueFalse", false)
                .Property("Point", new KEI.Infrastructure.Point(23, 45))
                .Build();

            var xmlString = XmlHelper.SerializeToString(SamplePropertyContainer);
            var obj = XmlHelper.DeserializeFromString<PropertyContainer>(xmlString);
        }

    }
}
