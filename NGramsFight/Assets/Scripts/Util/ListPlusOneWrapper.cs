using System.Collections;
using System.Collections.Generic;

namespace LibGameAI.Util
{
    public struct ListPlusOneWrapper<T> : IReadOnlyList<T>
    {
        public int Count { get; }
        private IReadOnlyList<T> list;
        private T plusOne;

        public ListPlusOneWrapper(IReadOnlyList<T> list, T plusOne)
        {
            this.list = list;
            this.plusOne = plusOne;
            Count = list.Count + 1;
        }

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

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T item in list) yield return item;
            yield return plusOne;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}