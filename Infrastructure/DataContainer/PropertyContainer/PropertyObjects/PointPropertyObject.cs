namespace KEI.Infrastructure
{
    internal class PointPropertyObject : PropertyObject<Point>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public PointPropertyObject(string name, Point value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => DataObjectType.Point;
    }
}
