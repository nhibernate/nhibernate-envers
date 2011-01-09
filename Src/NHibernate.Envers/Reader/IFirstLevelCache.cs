using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Reader
{
    /**
     * First level cache for versioned entities, versions reader-scoped. Each entity is uniquely identified by a
     * revision number and entity id.
     * @author Simon Duduica, created interface that separes the first level cache implementation.
     * Default implementation will be FirstLevelCache to be ported in the phase 2
     */
    public interface IFirstLevelCache {
        Object this[String entityName, long revision, Object id] { get; }
        void Add(String entityName, long revision, Object id, Object entity);
        bool Contains(String entityName, long revision, Object id);
    }
}
