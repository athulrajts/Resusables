using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEI.Infrastructure
{
    /// <summary>
    /// DataObject implementation for storing <see cref="TimeSpan"/>
    /// </summary>
    internal class TimeSpanDataObject : DataObject<TimeSpan>
    {
        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => DataObjectType.TimeSpan;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public TimeSpanDataObject(string name, TimeSpan value)
        {
            Name = name;
            Value = value;
        }
    }
}
