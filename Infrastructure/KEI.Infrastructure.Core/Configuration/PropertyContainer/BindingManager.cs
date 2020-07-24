using System;
using System.Linq;
using System.Timers;
using System.Collections.Generic;

namespace KEI.Infrastructure.Configuration
{
    internal class BindingManager
    {
        private static BindingManager instance;
        public static BindingManager Instance
            => instance ?? (instance = new BindingManager());

        private List<PropertyBinding> _bindings = new List<PropertyBinding>();
        private Timer timer = new Timer(TimeSpan.FromHours(1).TotalMilliseconds);
        internal BindingManager()
        {
            timer.Elapsed += Timer_Elapsed;
        }

        /// <summary>
        /// Clear all invalid bindings every hour incase user forgot to remove them
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_bindings.Count == 0)
                return;

            var expiredBindings = _bindings.Where(x => x.BindingTarget.IsAlive == false);

            foreach (var binding in expiredBindings)
            {
                RemoveBinding(binding);
            }
        }

        internal void AddBinding(PropertyBinding pb) => _bindings.Add(pb);
        internal PropertyBinding GetBinding(object target, string name) => _bindings.Find(x => x.BindingTarget == target && x.TargetProperty.Name == name);
        internal void RemoveBinding(PropertyBinding pb)
        {
            pb.Dispose();
            _bindings.Remove(pb);
        }
    }
}
