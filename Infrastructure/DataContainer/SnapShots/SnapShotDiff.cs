using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace KEI.Infrastructure
{
    /// <summary>
    /// Represents the difference between 2 <see cref="SnapShot"/>s
    /// </summary>
    public class SnapShotDiff : IEnumerable<SnapShotDiffItem>
    {
        // values
        private readonly SortedDictionary<string, SnapShotDiffItem> diffs = new();

        // first snapshot
        public SnapShot Left { get; }
        
        // second snapshot
        public SnapShot Right { get; }

        // name of all values
        public IEnumerable<string> Keys => diffs.Keys;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public SnapShotDiff(SnapShot left, SnapShot right)
        {
            Left = left;
            Right = right;

            CalculateDiff();
        }

        /// <summary>
        /// Find all the changes
        /// </summary>
        private void CalculateDiff()
        {
            var commonKeys = Left.Keys.Intersect(Right.Keys);
            var leftOnlyKeys = Left.Keys.Except(Right.Keys);
            var rightOnlyKeys = Right.Keys.Except(Left.Keys);

            // add left only
            foreach (var key in leftOnlyKeys)
            {
                AddDiffItem(key, Left[key].Type, Left[key].Value, null);
            }

            // add right only
            foreach (var key in rightOnlyKeys)
            {
               AddDiffItem(key, Right[key].Type, null, Right[key].Value);
            }

            // add common
            foreach (var key in commonKeys)
            {
                object value1 = Left[key].Value;
                object value2 = Right[key].Value;

                if (value1.Equals(value2) == false)
                {
                    AddDiffItem(key, Right[key].Type, value1, value2);
                }
            }
        }

        /// <summary>
        /// Get <see cref="SnapShotDiffItem"/> by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SnapShotDiffItem this[string name] => diffs[name];

        /// <summary>
        /// Adds a diff item
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        internal void AddDiffItem(string name, string type, object left, object right)
        {
            if(diffs.ContainsKey(name))
            {
                diffs[name].Left = left;
                diffs[name].Right = right;
            }
            else
            {
                diffs.Add(name, new SnapShotDiffItem 
                {
                    Left = left,
                    Right = right, 
                    DataObjectName = name,
                    DataObjectType = type
                });
            }
        }

        /// <summary>
        /// Enumeration support, can be used as ItemSource
        /// </summary>
        /// <returns></returns>
        public IEnumerator<SnapShotDiffItem> GetEnumerator() => diffs.Values.GetEnumerator();

        /// <summary>
        /// Enumeration support, can be used as ItemSource
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
