using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using NHibernate.Envers.Query.Order;
using NHibernate.Envers.Query.Criteria;
using NHibernate.Envers.Query.Projection;

namespace NHibernate.Envers.Query
{
    /**
     * @author Adam Warski (adam at warski dot org)
     * @see org.hibernate.Criteria
     */
    public interface IAuditQuery 
	{
		//rk: offer generic method here
        IList GetResultList();

		//rk: offer generic method here
        Object GetSingleResult();

        IAuditQuery Add(IAuditCriterion criterion);

        IAuditQuery AddProjection(IAuditProjection projection);

        IAuditQuery AddOrder(IAuditOrder order);

        IAuditQuery SetMaxResults(int maxResults);

	    IAuditQuery SetFirstResult(int firstResult);

        IAuditQuery SetCacheable(bool cacheable);

        IAuditQuery SetCacheRegion(String cacheRegion);

        IAuditQuery SetComment(String comment);

        IAuditQuery SetFlushMode(FlushMode flushMode);

        IAuditQuery SetCacheMode(CacheMode cacheMode);

        IAuditQuery SetTimeout(int timeout);

        IAuditQuery SetLockMode(LockMode lockMode);
    }
}
