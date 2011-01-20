using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Query.Order;
using NHibernate.Envers.Query.Criteria;
using NHibernate.Envers.Query.Projection;

namespace NHibernate.Envers.Query
{
	public interface IAuditQuery 
	{
		IList GetResultList();

		IList<T> GetResultList<T>();

		object GetSingleResult();

		T GetSingleResult<T>();

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
