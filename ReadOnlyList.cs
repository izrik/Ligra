using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Ligra
{
    public class ReadOnlyList<T> : IReadOnlyList<T>
    {
        public ReadOnlyList(IList<T> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            _source = source;
        }

        readonly IList<T> _source;

        #region IReadOnlyList implementation

        public int IndexOf(T item)
        {
            return _source.IndexOf(item);
        }

        public bool Contains(T item)
        {
            return _source.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _source.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        public T this[int index]
        {
            get { return _source[index]; }
        }

        public int Count
        {
            get { return _source.Count; }
        }

        #endregion
    }
}

