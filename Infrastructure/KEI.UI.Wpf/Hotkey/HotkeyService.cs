using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KEI.UI.Wpf.Hotkey
{
    public class HotkeyService : IHotkeyService
    {
        private readonly Dictionary<string, GestureCommand> _gestureCommands = new Dictionary<string, GestureCommand>();
        private readonly Dictionary<string, GestureCommand> _readonlyGestureCommands = new Dictionary<string, GestureCommand>();
        private InputBindingCollection _gestureProvider;

        public void AddGesture(GestureCommand command)
        {

            if (!_gestureCommands.ContainsKey(command.ID))
            {
                _gestureCommands.Add(command.ID, command);
            }

            _gestureProvider.Add(command.KeyBinding);

        }

        public void AddReadonlyGesture(GestureCommand command)
        {

            if (!_readonlyGestureCommands.ContainsKey(command.ID))
            {
                _readonlyGestureCommands.Add(command.ID, command);
            }

            _gestureProvider.Add(command.KeyBinding);

        }

        public void RemoveGesture(string uid)
        {
            InputBinding binding = null;
            if (_readonlyGestureCommands.ContainsKey(uid))
                binding = _readonlyGestureCommands[uid].KeyBinding;
            else if (_gestureCommands.ContainsKey(uid))
                binding = _gestureCommands[uid].KeyBinding;

            if (binding != null)
            {
                _gestureProvider.Remove(binding);
            }
        }



        public void SetGestureProvider(InputBindingCollection bindings)
        {
            _gestureProvider = bindings;
        }

        public void ReadConfig(string configPath)
        {
            throw new NotImplementedException();
        }

        public void StoreConfig(string configPath)
        {
            throw new NotImplementedException();
        }
    }
}
