using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Generator.Utilities
{
    public sealed class Deque<T> : IEnumerable<T>
    {
        private readonly List<T> _items;

        public Deque()
        {
            _items = new List<T>();
        }

        public void Append(T item)
        {
            _items.Add(item);
        }

        public T Pop()
        {
            var last = _items[_items.Count - 1];
            _items.RemoveAt(_items.Count - 1);
            return last;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
