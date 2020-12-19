using KEI.Infrastructure;
using KEI.Infrastructure.Prism;
using KEI.Infrastructure.Screen;
using KEI.UI.Wpf.Hotkey;
using Application.Production.Views;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Application.Production.ViewModels
{
    public enum Shape
    {
        Rectangle,
        Ellipse
    }

    public class TestClass
    {
        public int MyProp { get; set; } = 24;
        public string MyProperty { get; set; } = "Hello";
        public bool World { get; set; } = true;
    }

    [Screen(DisplayName = "Demos 3",
     Icon = Icon.Property16x,
     ScreenName = nameof(DemoScreen3))]
    public class DemoScreen3ViewModel : BaseScreenViewModel
    {
        public IPropertyContainer SamplePropertyContainer { get; set; }

        public DemoScreen3ViewModel(IHotkeyService hotkeyService) : base(hotkeyService)
        {
            var test = new List<TestClass> { new(), new(), new() };

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
                .Property("Container", test, SerializationFormat.Container)
                .Property("Xml", test, SerializationFormat.Xml)
                .Property("Json", test, SerializationFormat.Json)
                .Build();

            var xml = XmlHelper.SerializeToString(SamplePropertyContainer);
            //var obj = XmlHelper.DeserializeFromString<PropertyContainer>(xml);
        }

    }
}
