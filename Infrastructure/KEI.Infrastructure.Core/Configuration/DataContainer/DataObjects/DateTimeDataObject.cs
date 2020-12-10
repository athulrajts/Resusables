﻿using System;

namespace KEI.Infrastructure
{
    /// <summary>
    /// DataObject Implementation for storing <see cref="DateTime"/>
    /// </summary>
    internal class DateTimeDataObject : DataObject<DateTime>
    {
        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => "dt";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public DateTimeDataObject(string name, DateTime value)
        {
            Name = name;
            Value = value;
        }
    }
}
