using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy
{
	public class DictionaryProxy<K, V> : IDictionary<K, V>
	{
		private readonly Lazy<IDictionary<K, V>> _delegate;
		
		public DictionaryProxy(IInitializor initializor)
		{
			_delegate = new Lazy<IDictionary<K, V>>(() => (IDictionary<K, V>) initializor.Initialize());
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			return _delegate.Value.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(KeyValuePair<K, V> item)
		{
			_delegate.Value.Add(item);
		}

		public void Clear()
		{
			_delegate.Value.Clear();
		}

		public bool Contains(KeyValuePair<K, V> item)
		{
			return _delegate.Value.Contains(item);
		}

		public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			_delegate.Value.CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<K, V> item)
		{
			return _delegate.Value.Remove(item);
		}

		public int Count => _delegate.Value.Count;

		public bool IsReadOnly => _delegate.Value.IsReadOnly;

		public bool ContainsKey(K key)
		{
			return _delegate.Value.ContainsKey(key);
		}

		public void Add(K key, V value)
		{
			_delegate.Value.Add(key, value);
		}

		public bool Remove(K key)
		{
			return _delegate.Value.Remove(key);
		}

		public bool TryGetValue(K key, out V value)
		{
			return _delegate.Value.TryGetValue(key, out value);
		}

		public V this[K key]
		{
			get => _delegate.Value[key];
			set => _delegate.Value[key] = value;
		}

		public ICollection<K> Keys => _delegate.Value.Keys;

		public ICollection<V> Values => _delegate.Value.Values;
	}
}