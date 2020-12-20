using System;

namespace KEI.Infrastructure
{
    /// <summary>
    /// PropertyObject implementation for storing <see cref="TimeSpan"/>
    /// </summary>
    internal class TimeSpanPropertyObject : PropertyObject<TimeSpan>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public TimeSpanPropertyObject(string name, TimeSpan value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => DataObjectType.TimeSpan;
    }
}
