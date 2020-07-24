using System.Windows.Input;
using KEI.Infrastructure.Screen;

namespace Application.Production.Screen
{
    public class ScreenCommand
    {
        public string DisplayName { get; set; }
        public ICommand Command { get; set; }
        public object CommandParameter { get; set; }
        public Icon Icon { get; set; } = Icon.None16x;
    }
}
