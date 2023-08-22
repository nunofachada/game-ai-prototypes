/* Copyright (c) 2018-2023 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LibGameAI.Util
{
    /// <summary>
    /// A ring list with a specified size. Adding more elements to the list over
    /// its size will silently drop older elements.
    /// </summary>
    /// <typeparam name="T">The type of objects in the list.</typeparam>
    public class RingList<T> : IReadOnlyList<T>, ICollection<T>
    {
        /// <summary>
        /// Number of elements in the list.
        /// </summary>
        public int Count { get; private set; }

        /// <see cref="Count"/>
        int ICollection<T>.Count => Count;

        /// <summary>
        /// Is the list read-only (no, it's not).
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Maximum number of element the list can contain at the same time.
        /// </summary>
        public int Capacity { get; }

        // Current start index with respect to the internal support array
        private int startIndex;

        // Internal array supporting the ring list
        private readonly T[] items;

        /// <summary>
        /// Create a new ring list with the specified <paramref name="capacity/">.
        /// </summary>
        /// <param name="capacity">Maximum capacity of the ring list.</param>
        public RingList(int capacity)
        {
            startIndex = 0;
            Count = 0;
            this.Capacity = capacity;
            items = new T[capacity];
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to get or set.
        /// </param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown if index is greater than <paramref name="Count"/>.
        /// </exception>
        public T this[int index]
        {
            get
            {
                if (index > Count)
                {
                    throw new IndexOutOfRangeException(
                        $"Index {index} is out of range. {nameof(RingList<T>)} only contains {Count} items.");
                }
                else
                {
                    return items[ReIndex(index)];
                }
            }
            set
            {
                if (index > Count)
                {
                    throw new IndexOutOfRangeException(
                        $"Index {index} is out of range. {nameof(RingList<T>)} only contains {Count} items.");
                }
                else
                {
                    items[ReIndex(index)] = value;
                }
            }
        }

        /// <summary>
        /// Add an item to the end of the list. If list is already at maximum
        /// capacity, the first element will be silently removed.
        /// </summary>
        /// <param name="item">Item to add to the list.</param>
        public void Add(T item)
        {
            if (Count < Capacity)
            {
                items[Count] = item;
                Count++;
            }
            else
            {
                items[startIndex] = item;
                startIndex++;
                if (startIndex == Capacity) startIndex = 0;
            }
        }

        /// <summary>
        /// Clear list contents.
        /// </summary>
        public void Clear()
        {
            Array.Clear(items, 0, Capacity);
            startIndex = 0;
            Count = 0;
        }

        /// <summary>
        /// Determines whether the list contains a specific value.
        /// </summary>
        /// <param name="item">The item to locate in the list.</param>
        /// <returns>
        /// <c>true</c> if item is found in the list; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the list to an array, starting at a
        /// particular array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional array that is the
        /// destination of the elements copied from the list. The array must
        /// have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in <paramref name="array"/> at which copying
        /// begins.
        /// </param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < Count; i++)
            {
                array[arrayIndex + i] = items[ReIndex(i)];
            }
        }

        /// <summary>
        /// Unsupported operation.
        /// </summary>
        /// <param name="item">
        /// The item that would be removed from the list if this operation was
        /// supported.
        /// </param>
        /// <returns>Never returns anything, always throws an exception.</returns>
        /// <exception cref="InvalidOperationException">
        /// This exception is always thrown.
        /// </exception>
        public bool Remove(T item)
        {
            throw new InvalidOperationException(
                $"Cannot explicitly remove an item from a {nameof(RingList<T>)}.");
        }

        /// <summary>
        /// Returns an enumerator that iterates through the list elements.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the list elements.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return items[ReIndex(i)];
            }
        }

        /// <see cref="GetEnumerator"/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // Converts an arbitrary index to an index in the internal support array.
        private int ReIndex(int index)
        {
            if (index < 0)
            {
                index += Capacity;
            }
            else if (Count == Capacity)
            {
                index = (startIndex + index) % Capacity;
            }
            return index;
        }
    }
}