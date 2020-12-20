using System;

namespace KEI.Infrastructure
{
    /// <summary>
    /// PropertyObject Implementation for storing <see cref="DateTime"/>
    /// </summary>
    internal class DateTimePropertyObject : PropertyObject<DateTime>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public DateTimePropertyObject(string name, DateTime value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => DataObjectType.DateTime;
    }
}
