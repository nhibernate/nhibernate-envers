using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy
{
	[Serializable]
	public class DictionaryProxy<K, V> : IDictionary<K, V>
	{
		[NonSerialized]
		private readonly IInitializor _initializor;
		private IDictionary<K, V> _delegate;
		
		public DictionaryProxy(IInitializor initializor)
		{
			_initializor = initializor;
		}
		
		private void checkInit()
		{
			if (_delegate == null)
			{
				_delegate = (IDictionary<K, V>) _initializor.Initialize();
			}
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			checkInit();
			return _delegate.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			checkInit();
			return GetEnumerator();
		}

		public void Add(KeyValuePair<K, V> item)
		{
			checkInit();
			_delegate.Add(item);
		}

		public void Clear()
		{
			checkInit();
			_delegate.Clear();
		}

		public bool Contains(KeyValuePair<K, V> item)
		{
			checkInit();
			return _delegate.Contains(item);
		}

		public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			checkInit();
			_delegate.CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<K, V> item)
		{
			checkInit();
			return _delegate.Remove(item);
		}

		public int Count
		{
			get
			{
				checkInit();
				return _delegate.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				checkInit();
				return _delegate.IsReadOnly;
			}
		}

		public void Add(K key, V value)
		{
			checkInit();
			_delegate.Add(key, value);
		}

		public bool ContainsKey(K key)
		{
			checkInit();
			return _delegate.ContainsKey(key);
		}

		public bool Remove(K key)
		{
			checkInit();
			return _delegate.Remove(key);
		}

		public bool TryGetValue(K key, out V value)
		{
			checkInit();
			return _delegate.TryGetValue(key, out value);
		}

		public V this[K key]
		{
			get
			{
				checkInit();
				return _delegate[key];
			}
			set
			{
				checkInit();
				_delegate[key] = value;
			}
		}

		public ICollection<K> Keys
		{
			get
			{
				checkInit();
				return _delegate.Keys;
			}
		}

		public ICollection<V> Values
		{
			get
			{
				checkInit();
				return _delegate.Values;
			}
		}
	}
}