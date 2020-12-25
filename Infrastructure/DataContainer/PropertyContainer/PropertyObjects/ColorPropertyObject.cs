using System;

namespace KEI.Infrastructure
{
    /// <summary>
    /// PropertyObject implementation to store Color
    /// Application UI should convert to UI color object using RGB values
    /// </summary>
    public class ColorPropertyObject : PropertyObject<Color>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="color"></param>
        public ColorPropertyObject(string name, Color color)
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
