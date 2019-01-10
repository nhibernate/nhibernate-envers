using System;
using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy
{
	[Serializable]
	public class DictionaryProxy<K, V> : CollectionProxy<KeyValuePair<K, V>>, IDictionary<K, V>
	{
		public DictionaryProxy(IInitializor initializor) : base(initializor)
		{
		}

		public void Add(K key, V value)
		{
			GetCollection<IDictionary<K, V>>().Add(key, value);
		}

		public bool ContainsKey(K key)
		{
			return GetCollection<IDictionary<K, V>>().ContainsKey(key);
		}

		public bool Remove(K key)
		{
			return GetCollection<IDictionary<K, V>>().Remove(key);
		}

		public bool TryGetValue(K key, out V value)
		{
			return GetCollection<IDictionary<K, V>>().TryGetValue(key, out value);
		}

		public V this[K key]
		{
			get => GetCollection<IDictionary<K, V>>()[key];
			set => GetCollection<IDictionary<K, V>>()[key] = value;
		}

		public ICollection<K> Keys => GetCollection<IDictionary<K, V>>().Keys;

		public ICollection<V> Values => GetCollection<IDictionary<K, V>>().Values;
	}
}