using System;
using System.Collections.Generic;

namespace Dainty.UI
{
    public class ListStack<T>
    {
        private T[] _array;
        private int _size;
        private int _version;
        private static readonly T[] _EmptyArray = new T[0];

        public ListStack()
        {
            _array = _EmptyArray;
            _size = 0;
            _version = 0;
        }

        public ListStack(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException($"{nameof(capacity)} cannot be lower than 0");

            _array = new T[capacity];
            _size = 0;
            _version = 0;
        }

        public ListStack(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException($"{nameof(collection)} cannot be null");

            if (collection is ICollection<T> objs)
            {
                var count = objs.Count;
                _array = new T[count];
                objs.CopyTo(_array, 0);
                _size = count;
            }
            else
            {
                _size = 0;
                _array = new T[4];
                foreach (var obj in collection)
                    Push(obj);
            }
        }

        public int Count => _size;

        public void Clear()
        {
            Array.Clear(_array, 0, _size);
            _size = 0;
            ++_version;
        }

        public T Peek()
        {
            if (_size == 0)
                throw new InvalidOperationException("Stack is empty");

            return _array[_size - 1];
        }

        public T Pop()
        {
            if (_size == 0)
                throw new InvalidOperationException("Stack is empty");
            ++_version;
            var obj = _array[--_size];
            _array[_size] = default;
            return obj;
        }

        public void Push(T item)
        {
            if (_size == _array.Length)
            {
                var objArray = new T[_array.Length == 0 ? 4 : 2 * _array.Length];
                Array.Copy(_array, 0, objArray, 0, _size);
                _array = objArray;
            }

            _array[_size++] = item;
            ++_version;
        }

        public T RemoveAt(int index)
        {
            if (index < 0 || index >= _size)
                throw new ArgumentOutOfRangeException(nameof(index));

            var result = _array[index];
            for (var i = index + 1; i < _size; i++)
            {
                _array[i - 1] = _array[i];
            }

            --_size;
            ++_version;
            return result;
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _size)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return _array[index];
            }
            set
            {
                if (index < 0 || index >= _size)
                    throw new ArgumentOutOfRangeException(nameof(index));
                _array[index] = value;
            }
        }
    }
}