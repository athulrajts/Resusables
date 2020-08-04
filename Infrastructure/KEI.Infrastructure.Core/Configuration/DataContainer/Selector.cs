using System;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using Prism.Mvvm;
using KEI.Infrastructure.Types;

namespace KEI.Infrastructure.Configuration
{
    /// <summary>
    /// Container for Holding <see cref="Enum"/> Properties or, Properties whos
    /// values are restricted to a set of values.
    /// </summary>
    [XmlRoot("Property")]
    public class Selector : BindableBase
    {
        private string selectedItem;
        private List<string> options = new List<string>();

        /// <summary>
        /// Value of the Propery
        /// </summary>
        [XmlAttribute(AttributeName = "Value")]
        public string SelectedItem
        {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem, value); }
        }

        /// <summary>
        /// Allowed values the property Can have
        /// </summary>
        [XmlElement]
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

        /// <summary>
        /// Create an instance by specifiying the value and allowed values.
        /// If Value is not present inside allowed values, it'll be added internally
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selectedValue">Value</param>
        /// <param name="options"><see cref="Array"/> of allowed values</param>
        /// <returns></returns>
        public static Selector Create<T>(T selectedValue, params T[] options)
        {
            var ret = new Selector();
            ret.Option = options.Select(x => x.ToString()).ToList();
            ret.SelectedItem = selectedValue.ToString();
            if(!ret.Option.Contains(ret.SelectedItem))
            {
                ret.Option.Add(ret.SelectedItem);
            }
            ret.Type = new TypeInfo(typeof(T));

            return ret;
        }

        /// <summary>
        /// Create instance by specifiying allowed values
        /// Value will be first item in the allowed values by default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options">Allowed values</param>
        /// <returns></returns>
        public static Selector Create<T>(params T[] options)
        {
            var ret = new Selector();
            ret.Option = options.Select(x => x.ToString()).ToList();
            ret.SelectedItem = options[0].ToString();
            ret.Type = new TypeInfo(typeof(T));

            return ret;
        }

        public Selector Clone(string newValue)
        {
            var ret = new Selector();
            ret.Option = Option;
            ret.Type = Type;
            ret.SelectedItem = newValue;

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
