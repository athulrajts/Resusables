using System;
using System.Collections.Generic;
using KEI.Infrastructure.Types;
using System.Collections;

namespace KEI.Infrastructure
{
    /// <summary>
    /// Container for Holding <see cref="Enum"/> Properties or, Properties whos
    /// values are restricted to a set of values.
    /// </summary>
    public class Selector : BindableObject
    {
        private string selectedItem;
        private List<string> options = new List<string>();

        /// <summary>
        /// Value of the Propery
        /// </summary>
        public string SelectedItem
        {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem, value); }
        }

        /// <summary>
        /// Allowed values the property Can have
        /// </summary>
        public List<string> Option
        {
            get { return options; }
            set { SetProperty(ref options, value); }
        }

        /// <summary>
        /// Type of Property
        /// </summary>
        public TypeInfo Type { get; set; }

        /// <summary>
        /// Create instance from a <see cref="Enum"/>
        /// </summary>
        /// <param name="t"></param>
        public Selector(Enum t)
        {
            Option = new List<string>(Enum.GetNames(t.GetType()));
            SelectedItem = t.ToString();
            Type = new TypeInfo(t.GetType());
        }

        public Selector(string selectedValue, IList options)
        {
            foreach (var opt in options)
            {
                Option.Add(opt.ToString());
            }

            SelectedItem = selectedValue;

            Type = options[0]?.GetType();
        }

        public Selector Clone(string newValue)
        {
            var ret = new Selector
            {
                Option = Option,
                Type = Type,
                SelectedItem = newValue
            };

            return ret;
        }

        /// <summary>
        /// Default contructor to support XML serialization
        /// </summary>
        public Selector()
        {

        }

        public override string ToString() => SelectedItem;

        public static implicit operator string(Selector s) => s.SelectedItem;
    }
}
