using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Ligra
{
    public interface IReadOnlyList<T> : IEnumerable<T>
    {
        int IndexOf(T item);

        T this [int index] { get; }

        bool Contains(T item);

        void CopyTo(T[] array, int arrayIndex);

        int Count { get; }
    }
}

