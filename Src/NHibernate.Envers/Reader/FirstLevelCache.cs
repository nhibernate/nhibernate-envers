using System.Collections.Generic;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Reader
{
	/// <summary>
	/// First level cache for versioned entities, versions reader-scoped. Each entity is uniquely identified by a
	/// revision number and entity id.
	/// </summary>
	public class FirstLevelCache : IFirstLevelCache
	{
		private readonly IDictionary<Triple<string, long, object>, object> cache;

		public FirstLevelCache()
		{
			cache = new Dictionary<Triple<string, long, object>, object>();
		}

		public object this[string entityName, long revision, object id]
		{
			get
			{
				return cache[new Triple<string, long, object>(entityName, revision, id)];
			}
		}

		public void Add(string entityName, long revision, object id, object entity)
		{
			cache.Add(new Triple<string, long, object>(entityName, revision, id), entity);
		}

		public bool Contains(string entityName, long revision, object id)
		{
			return cache.ContainsKey(new Triple<string, long, object>(entityName, revision, id));
		}
	}
}
