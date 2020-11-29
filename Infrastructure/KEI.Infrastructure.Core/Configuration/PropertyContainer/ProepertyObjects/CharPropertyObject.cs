namespace KEI.Infrastructure
{
    /// <summary>
    /// PropertyObject implementation for <see cref="char"/>
    /// </summary>
    internal class CharPropertyObject : PropertyObject<char>
    {
        /// <summary>
        /// Implementation for <see cref="PropertyObject.Editor"/>
        /// </summary>
        public override EditorType Editor => EditorType.String;

        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => "char";

        /// <summary>
        /// Implementation for <see cref="DataObject.CanConvertFromString(string)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool CanConvertFromString(string value)
        {
            return char.TryParse(value, out _);
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.ConvertFromString(string)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFromString(string value)
        {
            return char.TryParse(value, out char tmp)
                ? tmp
                : null;
        }

        /// <summary>
        /// Implemenatation for <see cref="DataObject.OnStringValueChanged(string)"/>
        /// </summary>
        /// <param name="value"></param>
        protected override void OnStringValueChanged(string value)
        {
            if (char.TryParse(value, out _value))
            {
                RaisePropertyChanged(nameof(Value));
            }
        }
    }
}
