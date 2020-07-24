using System.Windows.Input;

namespace KEI.UI.Wpf.Hotkey
{
    public interface IHotkeyService
    {
        void SetGestureProvider(InputBindingCollection bindings);
        void ReadConfig(string configPath);
        void StoreConfig(string configPath);
        void AddGesture(GestureCommand command);
        void AddReadonlyGesture(GestureCommand command);
        void RemoveGesture(string uid);

    }
}
