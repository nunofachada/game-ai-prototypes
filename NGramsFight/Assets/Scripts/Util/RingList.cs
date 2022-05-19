using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LibGameAI.Util
{
    public class RingList<T> : IReadOnlyList<T>, ICollection<T>
    {
        public int Count => count;
        int ICollection<T>.Count => Count;

        public bool IsReadOnly => false;

        private int startIndex;
        private int count;
        private readonly int size;
        private readonly T[] items;

        public RingList(int size)
        {
            startIndex = 0;
            count = 0;
            this.size = size;
            items = new T[size];
        }

        public T this[int index]
        {
            get
            {
                if (index > count)
                {
                    throw new IndexOutOfRangeException(
                        $"Index {index} is out of range. {nameof(RingList<T>)} only contains {count} items.");
                }
                else
                {
                    return items[ReIndex(index)];
                }
            }
        }

        public void Add(T item)
        {
            if (count < size)
            {
                items[count] = item;
                count++;
            }
            else
            {
                items[startIndex] = item;
                startIndex++;
                if (startIndex == size) startIndex = 0;
            }

        }

        public void Clear()
        {
            Array.Clear(items, 0, size);
            startIndex = 0;
            count = 0;
        }

        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < count; i++)
            {
                array[arrayIndex + i] = items[ReIndex(i)];
            }
        }

        public bool Remove(T item)
        {
            throw new InvalidOperationException(
                $"Cannot explicitly remove an item from a {nameof(RingList<T>)}.");
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < count; i++)
            {
                yield return items[ReIndex(i)];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private int ReIndex(int index)
        {
            if (index < 0)
            {
                index += size;
            }
            else if (count == size)
            {
                index = (startIndex + index) % size;
            }
            return index;
        }
    }
}