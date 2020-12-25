namespace KEI.Infrastructure
{
    /// <summary>
    /// PropertyObject implementation for <see cref="bool"/>
    /// </summary>
    internal class BoolPropertyObject : PropertyObject<bool>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public BoolPropertyObject(string name, bool value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => DataObjectType.Boolean;
    }
}
