using KEI.Infrastructure.Prism;
using KEI.Infrastructure.Screen;
using KEI.UI.Wpf;
using KEI.UI.Wpf.Hotkey;
using Application.Production.Views;

namespace Application.Production.ViewModels
{
    [Screen(DisplayName = "Alarms", 
            Icon = Icon.UserWarning16x,
            ScreenName = nameof(AlarmScreen))]
    public class AlarmScreenViewModel : BaseScreenViewModel<AlarmScreen>
    {
        public AlarmScreenViewModel(IHotkeyService hotkeyService) : base(hotkeyService)
        {
        }

        public override string DisplayName { get; set; } = @"Alarm";
        public override Icon Icon { get; set; } = Icon.UserWarning16x;

    }
}
