using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Envers.Tools
{
    public class EmptyDictionary<TKey, TValue>: IDictionary<TKey, TValue>{

        public class EmptyEnumerator<T>: IEnumerator<T>{
            public static EmptyEnumerator<T> Instance = new EmptyEnumerator<T>();
            private EmptyEnumerator(){}
            public void Dispose() {}
            public bool MoveNext(){ return false; }
            public void Reset(){}
            public T Current{get { throw new IndexOutOfRangeException(); }}
            object IEnumerator.Current{get { throw new IndexOutOfRangeException(); }}
        }

        public static EmptyDictionary<TKey, TValue> Instance = new EmptyDictionary<TKey, TValue>();

        private EmptyDictionary(){}

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return EmptyEnumerator<KeyValuePair<TKey, TValue>>.Instance;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException("An EmptyDictionary should remain empty!");
        }

        public void Clear()
        {
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotSupportedException("An EmptyDictionary should remain empty!");
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return false;
        }

        public int Count
        {
            get { return 0; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool ContainsKey(TKey key)
        {
            return false;
        }

        public void Add(TKey key, TValue value)
        {
            throw new NotSupportedException("An EmptyDictionary should remain empty!");
        }

        public bool Remove(TKey key)
        {
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            return false;
        }

        public TValue this[TKey key]
        {
            get { throw new KeyNotFoundException("An EmptyDictionary is empty!"); }
            set { throw new NotSupportedException("An EmptyDictionary should remain empty!"); }
        }

        public ICollection<TKey> Keys
        {
            get { throw new NotSupportedException("An EmptyDictionary is empty!"); }
        }

        public ICollection<TValue> Values
        {
            get { throw new NotSupportedException("An EmptyDictionary is empty!"); }
        }
    }
}