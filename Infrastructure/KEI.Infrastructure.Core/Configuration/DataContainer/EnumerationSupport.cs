using System;
using System.Collections;
using System.Collections.Generic;

namespace KEI.Infrastructure.Configuration
{
    public class BasicEnumerator<T> : IEnumerator<T>
    {
        private IList<T> _dataObjects;
        private int position = -1;
        public BasicEnumerator(IList<T> dataObjects)
        {
            _dataObjects = dataObjects;
        }
        public T Current
        {
            get
            {
                try
                {
                    return _dataObjects[position];
                }

                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose() => Reset();

        public bool MoveNext() => ++position < _dataObjects.Count;

        public void Reset() => position = -1;
    }

}
