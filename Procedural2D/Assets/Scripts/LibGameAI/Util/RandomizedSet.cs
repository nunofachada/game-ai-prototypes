/* Copyright (c) 2018-2025 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System;
using System.Collections.Generic;

namespace LibGameAI.Util
{

    /// <summary>
    /// Fast removal of random element and fast insertion of new elements.
    /// </summary>
    /// <typeparam name="T">The type of element this collection contains.</typeparam>
    public class RandomizedSet<T>
    {
        private readonly List<T> items;
        private readonly Dictionary<T, int> indexMap;
        private readonly Random random;

        public RandomizedSet(int? seed = null)
        {
            items = new List<T>();
            indexMap = new Dictionary<T, int>();
            random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        public bool Add(T item)
        {
            if (indexMap.ContainsKey(item))
                return false;

            indexMap[item] = items.Count;
            items.Add(item);
            return true;
        }

        public bool Remove(T item)
        {
            if (!indexMap.TryGetValue(item, out int index))
                return false;

            // Swap with the last element
            int lastIndex = items.Count - 1;
            T lastItem = items[lastIndex];

            items[index] = lastItem;
            indexMap[lastItem] = index;

            // Remove the last item
            items.RemoveAt(lastIndex);
            indexMap.Remove(item);
            return true;
        }

        public T GetRandom(bool remove = false)
        {
            if (items.Count == 0)
                throw new InvalidOperationException("Collection is empty.");

            int randomIndex = random.Next(items.Count);
            T item = items[randomIndex];
            if (remove) Remove(item);
            return item;
        }

        public bool Contains(T item) => indexMap.ContainsKey(item);

        public int Count => items.Count;

        public void Clear()
        {
            items.Clear();
            indexMap.Clear();
        }
    }

}