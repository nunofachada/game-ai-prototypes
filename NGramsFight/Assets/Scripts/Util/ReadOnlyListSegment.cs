// Copyright (c) 2022 Nuno Fachada
// Distributed under the MIT License (See accompanying file LICENSE or copy
// at http://opensource.org/licenses/MIT)

using System;
using System.Collections;
using System.Collections.Generic;

namespace LibGameAI.Util
{
    /// <summary>
    /// An efficient read-only wrapper for viewing a list segment as a list.
    /// </summary>
    /// <typeparam name="T">The type of objects in the wrapped list.</typeparam>
    public struct ReadOnlyListSegment<T> : IReadOnlyList<T>
    {
        // Wrapped list.
        private IReadOnlyList<T> innerList;

        // Index, with respect to the wrapped list, of first element of the list
        // segment.
        private int offset;

        // Number of elements in the wrapped list.
        private int innerListSize;

        /// <summary>
        /// Number of items in the list segment.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to get.
        /// </param>
        /// <returns>The element at the specified index.</returns>
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

        /// <summary>
        /// Create a new read-only list segment.
        /// </summary>
        /// <param name="innerList">Wrapped list.</param>
        /// <param name="offset">
        /// Index, with respect to the wrapped list, of the first element in the
        /// list segment.
        /// </param>
        /// <param name="count">Number of items in the list segment.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <paramref name="innerList"/> is <c>null</c> or if
        /// <paramref name="innerList"/> has less than <paramref name="offset"/> +
        /// <paramref name="count"/> elements.
        /// </exception>
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

        /// <summary>
        /// Returns an enumerator that iterates through the list segment.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the list segment.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        /// <see cref="GetEnumerator"/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}