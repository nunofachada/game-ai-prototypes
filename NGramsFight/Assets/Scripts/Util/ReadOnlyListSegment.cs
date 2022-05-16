using System;
using System.Collections;
using System.Collections.Generic;

namespace LibGameAI.Util
{
    public struct ReadOnlyListSegment<T> : IReadOnlyList<T>
    {
        private IReadOnlyList<T> innerList;
        private int offset;
        private int innerListSize;

        public T this[int index] {
            get
            {
                if (innerList.Count != innerListSize)
                {
                    throw new InvalidOperationException(
                        "Inner list size was modified while mapped by this list segment");
                }

                if (index >= Count || index < 0)
                {
                    throw new IndexOutOfRangeException(
                        $"Index {index} is out of the list segment's range.");
                }
                return innerList[offset + index];
            }
        }

        public int Count { get; }

        public ReadOnlyListSegment(IReadOnlyList<T> innerList, int offset, int count)
        {
            if (innerList is null)
            {
                throw new InvalidOperationException("`innerList` cannot be null");
            }

            if (innerList.Count < offset + count)
            {
                throw new InvalidOperationException(
                    "`innerList` not large enough for the given `offset` and `count`");
            }

            innerListSize = innerList.Count;
            this.innerList = innerList;
            this.offset = offset;
            Count = count;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}