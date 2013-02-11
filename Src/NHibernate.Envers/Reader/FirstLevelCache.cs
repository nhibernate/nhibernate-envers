using System;
using System.Collections.Generic;

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
		private const string logAddEntityName =
			"Adding entityName on First Level Cache (primaryKey:{0} - revision:{1} - entity:{2} -> entityName:{3})";
		private const string logHitEntityName =
			"Successfully resolving entityName from first level cache (primaryKey:{0} - revision:{1} - entity:{2})";

		//cache for resolve an object for a given id, revision and entityName.
		private readonly IDictionary<Tuple<string, long, object>, object> cache;

		//used to resolve the entityName for a given id, revision and entity.
		private readonly IDictionary<Tuple<object, long, object>, string> entityNameCache;

		public FirstLevelCache()
		{
			cache = new Dictionary<Tuple<string, long, object>, object>();
			entityNameCache = new Dictionary<Tuple<object, long, object>, string>();
		}

		public void Add(string entityName, long revision, object id, object entity)
		{
			if(log.IsDebugEnabled)
				log.DebugFormat(logAdd, id, revision, entityName);
			cache.Add(new Tuple<string, long, object>(entityName, revision, id), entity);
		}

		public bool TryGetValue(string entityName, long revision, object id, out object value)
		{
			if (cache.TryGetValue(new Tuple<string, long, object>(entityName, revision, id), out value))
			{
				if(log.IsDebugEnabled)
					log.DebugFormat(logHit, id, revision, entityName);
				return true;
			}
			return false;
		}

		public void AddEntityName(object id, long revision, object entity, string entityName)
		{
			if (log.IsDebugEnabled)
				log.DebugFormat(logAddEntityName, id, revision, entity.GetType().FullName, entityName);
			entityNameCache.Add(new Tuple<object, long, object>(id, revision, entity), entityName);
		}

		public bool TryGetEntityName(object id, long revision, object entity, out string entityName)
		{
			if (entityNameCache.TryGetValue(new Tuple<object, long, object>(id, revision, entity), out entityName))
			{
				if(log.IsDebugEnabled)
					log.DebugFormat(logHitEntityName, id, revision, entity.GetType().FullName);
				return true;
			}
			return false;
		}
	}
}
