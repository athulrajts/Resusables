namespace KEI.Infrastructure
{
    /// <summary>
    /// PropertyObject Implementation for <see cref="byte"/>
    /// </summary>
    internal class BytePropertyObject : PropertyObject<byte>
    {
        /// <summary>
        /// Implementation for <see cref="PropertyObject.Editor"/>
        /// </summary>
        public override EditorType Editor => EditorType.String;

        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => "byte";

        /// <summary>
        /// Implementation for <see cref="DataObject.CanConvertFromString(string)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool CanConvertFromString(string value)
        {
            return byte.TryParse(value, out _);
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.ConvertFromString(string)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFromString(string value)
        {
            return byte.TryParse(value, out byte tmp)
                ? tmp
                : null;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.OnStringValueChanged(string)"/>
        /// </summary>
        /// <param name="value"></param>
        protected override void OnStringValueChanged(string value)
        {
            if(byte.TryParse(value, out _value))
            {
                RaisePropertyChanged(nameof(Value));
            }
        }
    }
}
