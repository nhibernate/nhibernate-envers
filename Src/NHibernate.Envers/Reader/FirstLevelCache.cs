using System.Collections.Generic;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Reader
{
	/// <summary>
	/// First level cache for versioned entities, versions reader-scoped. Each entity is uniquely identified by a
	/// revision number and entity id.
	/// </summary>
	public class FirstLevelCache
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(FirstLevelCache));
		private const string logAdd =
			"Adding entity to first level cache (Primary key: {0}, Revision: {1}, Entity name: {2})";
		private const string logHit =
			"Succesfully resolving object from first level cache (Primary key: {0}, Revision: {1}, Entity name: {2})";

		private readonly IDictionary<Triple<string, long, object>, object> cache;


		public FirstLevelCache()
		{
			cache = new Dictionary<Triple<string, long, object>, object>();
		}

		public void Add(string entityName, long revision, object id, object entity)
		{
			if(log.IsDebugEnabled)
				log.DebugFormat(logAdd, id, revision, entityName);
			cache.Add(new Triple<string, long, object>(entityName, revision, id), entity);
		}

		public bool TryGetValue(string entityName, long revision, object id, out object value)
		{
			if (cache.TryGetValue(new Triple<string, long, object>(entityName, revision, id), out value))
			{
				if(log.IsDebugEnabled)
					log.DebugFormat(logHit, id, revision, entityName);
				return true;
			}
			return false;
		}
	}
}
