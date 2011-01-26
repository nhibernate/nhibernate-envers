using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy
{
	[Serializable]
	public class MapProxy<K, V> : IDictionary<K,V>
	{
		private readonly IInitializor<IDictionary<K,V>> _initializor;
		private IDictionary<K, V> delegat;

		public MapProxy(IInitializor<IDictionary<K, V>> initializor)
		{
			_initializor = initializor;
		}


		private void checkInit()
		{
			if (delegat == null)
			{
				delegat = _initializor.Initialize();
			}
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			checkInit();
			return delegat.GetEnumerator();
		}

		public void Add(KeyValuePair<K, V> item)
		{
			checkInit();
			delegat.Add(item);
		}

		public void Clear()
		{
			checkInit();
			delegat.Clear();
		}

		public bool Contains(KeyValuePair<K, V> item)
		{
			checkInit();
			return delegat.Contains(item);
		}

		public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			checkInit();
			delegat.CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<K, V> item)
		{
			checkInit();
			return delegat.Remove(item);
		}

		public int Count
		{
			get
			{
				checkInit();
				return delegat.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				checkInit();
				return delegat.IsReadOnly;
			}
		}

		public bool ContainsKey(K key)
		{
			checkInit();
			return delegat.ContainsKey(key);
		}

		public void Add(K key, V value)
		{
			checkInit();
			delegat.Add(key, value);
		}

		public bool Remove(K key)
		{
			checkInit();
			return delegat.Remove(key);
		}

		public bool TryGetValue(K key, out V value)
		{
			checkInit();
			return delegat.TryGetValue(key, out value);
		}

		public V this[K key]
		{
			get
			{
				checkInit();
				return delegat[key];
			}
			set
			{
				checkInit();
				delegat[key] = value;
			}
		}

		public ICollection<K> Keys
		{
			get
			{
				checkInit();
				return delegat.Keys;
			}
		}

		public ICollection<V> Values
		{
			get
			{
				checkInit();
				return delegat.Values;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			checkInit();
			return GetEnumerator();
		}
	}
}