namespace KEI.Infrastructure
{
    /// <summary>
    /// PropertyObject Implementation for <see cref="byte"/>
    /// </summary>
    internal class BytePropertyObject : PropertyObject<byte>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public BytePropertyObject(string name, byte value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => DataObjectType.Byte;

    }
}
