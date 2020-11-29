﻿namespace KEI.Infrastructure
{
    /// <summary>
    /// PropertyObject implementation for <see cref="bool"/>
    /// </summary>
    internal class BoolPropertyObject : PropertyObject<bool>
    {
        /// <summary>
        /// Implementation for <see cref="PropertyObject.Editor"/>
        /// </summary>
        public override EditorType Editor => EditorType.Bool;

        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => "bool";

        /// <summary>
        /// Implementation for <see cref="DataObject.CanConvertFromString(string)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool CanConvertFromString(string value)
        {
            return bool.TryParse(value, out _);
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.ConvertFromString(string)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFromString(string value)
        {
            return bool.TryParse(value, out bool tmp)
                ? tmp
                : null;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.OnStringValueChanged(string)"/>
        /// </summary>
        /// <param name="value"></param>
        protected override void OnStringValueChanged(string value)
        {
            if(bool.TryParse(value, out _value))
            {
                RaisePropertyChanged(nameof(Value));
            }

        }
    }
}
