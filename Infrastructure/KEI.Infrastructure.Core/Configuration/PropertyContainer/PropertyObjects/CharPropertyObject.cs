namespace KEI.Infrastructure
{
    /// <summary>
    /// PropertyObject implementation for <see cref="char"/>
    /// </summary>
    internal class CharPropertyObject : PropertyObject<char>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public CharPropertyObject(string name, char value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => DataObjectType.Char;

    }
}
