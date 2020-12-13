using KEI.Infrastructure;
using KEI.Infrastructure.Prism;
using KEI.Infrastructure.Screen;
using KEI.UI.Wpf.Hotkey;
using Application.Production.Views;

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
                .Build();
        }

    }
}
