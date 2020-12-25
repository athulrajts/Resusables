namespace KEI.Infrastructure
{
    /// <summary>
    /// PropertyObject implementation for <see cref="string"/>
    /// </summary>
    internal class StringPropertyObject : PropertyObject<string>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public StringPropertyObject(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => DataObjectType.String;

        /// <summary>
        /// Implementation for <see cref="DataObject.ConvertFromString(string)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFromString(string value)
        {
            return value;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.OnStringValueChanged(string)"/>
        /// </summary>
        /// <param name="value"></param>
        protected override void OnStringValueChanged(string value)
        {
            _value = value;
            RaisePropertyChanged(nameof(Value));
        }
    }
}
