namespace KEI.Infrastructure
{
    /// <summary>
    /// DataObject implmementation to store Color data
    /// </summary>
    internal class ColorDataObject : DataObject<Color>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="color"></param>
        public ColorDataObject(string name, Color color)
        {
            Name = name;
            Value = color;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => DataObjectType.Color;
    }
}
