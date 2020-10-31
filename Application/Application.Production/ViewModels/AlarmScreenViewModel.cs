using KEI.Infrastructure.Prism;
using KEI.Infrastructure.Screen;
using KEI.UI.Wpf.Hotkey;
using Application.Production.Views;

namespace Application.Production.ViewModels
{
    [Screen(DisplayName = "Alarms", 
            Icon = Icon.UserWarning16x,
            ScreenName = nameof(AlarmScreen))]
    public class AlarmScreenViewModel : BaseScreenViewModel
    {
        public AlarmScreenViewModel(IHotkeyService hotkeyService) : base(hotkeyService)
        {
        }
    }
}
