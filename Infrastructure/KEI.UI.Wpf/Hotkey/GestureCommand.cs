using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;

namespace KEI.UI.Wpf.Hotkey
{
    public class GestureCommand
    {

        public string DisplayName { get; set; }

        public Key Key { get; set; }
        
        public ModifierKeys ModifierKeys { get; set; }

        public string ID { get; set; }

        [XmlIgnore]
        public KeyBinding KeyBinding { get; set; }

        public GestureCommand(string displayName, KeyBinding keyBinding)
        {
            DisplayName = displayName;
            KeyBinding = keyBinding;
        }

    }

    public class GestureCache
    {
        private static Dictionary<string, GestureCommand> _factory = new Dictionary<string, GestureCommand>();

        public static GestureCommand GetGesture(string displayName, ICommand command, object commandParameter, Key key, ModifierKeys modifierKeys)
        {
            var uid = $"{Assembly.GetCallingAssembly().GetName().Name}.{displayName}";

            if (_factory.ContainsKey(uid))
                return _factory[uid];

            var keyBinding = new KeyBinding { Key = key, Modifiers = modifierKeys, Command = command, CommandParameter = commandParameter };

            var newGesture = new GestureCommand(displayName, keyBinding) { ID = uid };

            _factory.Add(uid, newGesture);

            return newGesture;
        }
    }


}
