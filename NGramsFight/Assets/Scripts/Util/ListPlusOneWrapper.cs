// Copyright (c) 2022 Nuno Fachada
// Distributed under the MIT License (See accompanying file LICENSE or copy
// at http://opensource.org/licenses/MIT)

using System.Collections;
using System.Collections.Generic;

namespace LibGameAI.Util
{
    /// <summary>
    /// An efficient wrapper for a list plus one element.
    /// </summary>
    /// <typeparam name="T">Type of items in the collection.</typeparam>
    public struct ListPlusOneWrapper<T> : IReadOnlyList<T>
    {
        // Wrapped collection
        private IReadOnlyList<T> list;

        // Extra element
        private T plusOne;

        /// <summary>
        /// Number of items in the list (equal to the number of items in the
        /// wrapped list plus one).
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Create a new wrapper for a list plus one element.
        /// </summary>
        /// <param name="list">List to be wrapped.</param>
        /// <param name="plusOne">
        /// Extra element to apparently add to the list.
        /// </param>
        public ListPlusOneWrapper(IReadOnlyList<T> list, T plusOne)
        {
            this.list = list;
            this.plusOne = plusOne;
            Count = list.Count + 1;
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to get.
        /// </param>
        /// <returns>The element at the specified index.</returns>
        public T this[int index]
        {
            get
            {
                if (index < list.Count)
                {
                    return list[index];
                }
                else
                {
                    return plusOne;
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the wrapped list plus
        /// the additional element.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the list plus the
        /// additional element.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (T item in list) yield return item;
            yield return plusOne;
        }

        /// <see cref="GetEnumerator"/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}