using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Envers.Tools
{
    public class DictionaryWrapper<TKey, TValue>: IDictionary<TKey, TValue>
    {
        private readonly IDictionary dictionary;

        private DictionaryWrapper(IDictionary dic)
        {
            dictionary = dic;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new EnumeratorWrapper<KeyValuePair<TKey, TValue>>(dictionary.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            dictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return dictionary.Contains(item.Key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            dictionary.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            try
            {
                dictionary.Remove(item.Key);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public int Count
        {
            get { return dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return dictionary.IsReadOnly; }
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.Contains(key);
        }

        public void Add(TKey key, TValue value)
        {
            dictionary.Add(key, value);
        }

        public bool Remove(TKey key)
        {
            try
            {
                dictionary.Remove(key);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            try
            {
                value = (TValue)dictionary[key];
                return true;
            }
            catch (Exception)
            {
                value = default(TValue);
                return false;
            }
        }

        public TValue this[TKey key]
        {
            get { return (TValue)dictionary[key]; }
            set { dictionary[key] = value; }
        }

        public ICollection<TKey> Keys
        {
            get { return new CollectionWrapper<TKey>(dictionary.Keys); }
        }

        public ICollection<TValue> Values
        {
            get { return new CollectionWrapper<TValue>(dictionary.Values); }
        }

        public static IDictionary<TKey, TValue> Wrap(IDictionary o)
        {
            if (o == null) return null;
            return new DictionaryWrapper<TKey, TValue>(o);
        }
    }
}
