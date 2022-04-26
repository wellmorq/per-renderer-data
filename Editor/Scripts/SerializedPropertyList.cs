using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Wellmor.PerRendererData
{
    public class SerializedPropertyList<T> : IList<T> where T : class
    {
        public struct SerializedPropertyEnumerator : IEnumerator<T>, IDisposable, IEnumerator
        {
            private readonly SerializedPropertyList<T> _list;
            private int _index;

            public T Current { get; private set; }

            internal SerializedPropertyEnumerator(SerializedPropertyList<T> list)
            {
                _index = 0;
                _list = list;
                Current = default;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_index >= _list.Count) return MoveNextRare();
                Current = _list[_index];
                ++_index;
                return true;
            }

            private bool MoveNextRare()
            {
                _index = _list.Count + 1;
                Current = default;
                return false;
            }


            object IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || _index == _list.Count + 1) throw new InvalidOperationException();
                    return Current;
                }
            }

            void IEnumerator.Reset()
            {
                _index = 0;
                Current = default;
            }
        }
        
        private readonly SerializedProperty _property;

        public int Count => _property.arraySize;
        public bool IsReadOnly => false;

        public T this[int index]
        {
            get => (T)_property.GetArrayElementAtIndex(index).managedReferenceValue;
            set => _property.GetArrayElementAtIndex(index).managedReferenceValue = value;
        }
        
        public SerializedProperty GetRaw(int index) => _property.GetArrayElementAtIndex(index);

        public SerializedPropertyList(SerializedProperty prop)
        {
            _property = prop;
            if (_property == null) throw new ArgumentNullException();
        }
        
        public SerializedPropertyEnumerator GetEnumerator() => new(this);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new SerializedPropertyEnumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => new SerializedPropertyEnumerator(this);
        
        public void Add(T item)
        {
            Insert(_property.arraySize, item);
        }

        public bool Remove(T item)
        {
            if (!Contains(item)) return false;

            _property.DeleteArrayElementAtIndex(IndexOf(item));
            return true;
        }

        public void Clear()
        {
            _property.ClearArray();
        }

        public bool Contains(T item)
        {
            var equalityComparer = EqualityComparer<T>.Default;
            for (var index = 0; index < _property.arraySize; ++index)
            {
                if (equalityComparer.Equals(this[index], item))
                    return true;
            }

            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Resize(ref array, _property.arraySize);

            for (var i = 0; i < _property.arraySize; i++)
            {
                array[i] = (T)_property.GetArrayElementAtIndex(i).managedReferenceValue;
            }
        }

        public int IndexOf(T other)
        {
            var equalityComparer = EqualityComparer<T>.Default;

            for (var i = 0; i < _property.arraySize; i++)
            {
                var item = (T)_property.GetArrayElementAtIndex(i).managedReferenceValue;
                if (equalityComparer.Equals(item, other)) return i;
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            _property.InsertArrayElementAtIndex(index);
            this[index] = item;
        }

        public void RemoveAt(int index)
        {
            _property.DeleteArrayElementAtIndex(index);
        }
    }
}