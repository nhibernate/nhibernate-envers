using System;
using System.Collections.Generic;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Reader
{
    /// <summary>
    /// First level cache for versioned entities, versions reader-scoped. Each entity is uniquely identified by a
    /// revision number and entity id.
    /// </summary>
    public class FirstLevelCache: IFirstLevelCache 
    {
        private readonly IDictionary<Triple<String, long, Object>, Object> cache;

        public FirstLevelCache() 
        {
            cache = new Dictionary<Triple<string, long, object>, object>();
        }

        public object this[string entityName, long revision, object id] 
        { 
            get
            {
                return cache[Triple<string, long, object>.Make(entityName, revision, id)];
            }
        }

        public void Add(string entityName, long revision, object id, object entity) 
        {
            cache.Add(Triple<string, long, object>.Make(entityName, revision, id), entity);
        }

        public bool Contains(string entityName, long revision, object id) 
        {
            return cache.ContainsKey(Triple<string, long, object>.Make(entityName, revision, id));
        }
    }
}
