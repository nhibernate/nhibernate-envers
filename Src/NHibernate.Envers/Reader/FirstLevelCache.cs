using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Reader
{
    /**
     * First level cache for versioned entities, versions reader-scoped. Each entity is uniquely identified by a
     * revision number and entity id.
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public class FirstLevelCache: IFirstLevelCache {
        private readonly IDictionary<Triple<String, long, Object>, Object> cache;

        public FirstLevelCache() {
            cache = Toolz.NewDictionary<Triple<String, long, Object>, Object>();
        }

        public Object this[String entityName, long revision, Object id] { 
            get
            {
                return cache[Triple<String, long, Object>.Make(entityName, revision, id)];
            }
        }

        public void Add(String entityName, long revision, Object id, Object entity) {
            cache.Add(Triple<String, long, Object>.Make(entityName, revision, id), entity);
        }

        public bool Contains(String entityName, long revision, Object id) {
            return cache.ContainsKey(Triple<String, long, Object>.Make(entityName, revision, id));
        }
    }
}
