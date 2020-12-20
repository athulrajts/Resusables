using System;
using System.ComponentModel;
using System.Globalization;

namespace KEI.Infrastructure
{

    internal class PointDataObject : PropertyObject<Point>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public PointDataObject(string name, Point value)
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
