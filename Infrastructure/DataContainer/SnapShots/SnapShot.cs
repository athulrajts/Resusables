using System.Collections;
using System.Collections.Generic;

namespace KEI.Infrastructure
{
    /// <summary>
    /// Represents state of <see cref="IDataContainer"/>
    /// will not update when <see cref="IDataContainer"/> changes.
    /// </summary>
    public class SnapShot : IEnumerable<SnapShotItem>
    {
        // store all values
        private SortedDictionary<string, SnapShotItem> values = new();

        // get names of all values
        public IEnumerable<string> Keys => values.Keys;

        // indexer
        public SnapShotItem this[string key] => values[key];

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dc"></param>
        public SnapShot(IDataContainer dc)
        {
            PopulateValues(dc);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SnapShot() { }

        /// <summary>
        /// Add item to snapshot
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public void Add(string key, string type, object value)
        {
            values.Add(key, new SnapShotItem
            {
                Name = key,
                Type = type,
                Value = value 
            });
        }

        /// <summary>
        /// Update value is snapshot
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Update(string key, object value)
        {
            values[key].Value = value;
        }

        /// <summary>
        /// Initialize SnapShot from a <see cref="IDataContainer"/>
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="parent"></param>
        private void PopulateValues(IDataContainer dc, string parent="")
        {
            foreach (DataObject item in dc)
            {
                if (item.GetValue() is IDataContainer idc)
                {
                    string fullname = string.IsNullOrEmpty(parent) ? idc.Name : $"{parent}.{idc.Name}";
                    PopulateValues(idc, fullname);
                }
                else
                {
                    string key = string.IsNullOrEmpty(parent) ? item.Name : $"{parent}.{item.Name}";
                    values.Add(key, new SnapShotItem(item));
                }

            }
        }

        /// <summary>
        /// Enumeration support, can be used as ItemSource
        /// </summary>
        /// <returns></returns>
        public IEnumerator<SnapShotItem> GetEnumerator() => values.Values.GetEnumerator();

        /// <summary>
        /// Enumeration support, can be used as ItemSource
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        /// <summary>
        /// Binary - ("Minus") overload to compare 2 snapshots
        /// </summary>
        /// <param name="operand1"></param>
        /// <param name="operand2"></param>
        /// <returns></returns>
        public static SnapShotDiff operator -(SnapShot operand1, SnapShot operand2) => new SnapShotDiff(operand1, operand2);

    }
}
