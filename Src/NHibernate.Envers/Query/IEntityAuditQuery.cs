using System.Collections.Generic;
using NHibernate.Envers.Query.Criteria;
using NHibernate.Envers.Query.Order;

namespace NHibernate.Envers.Query
{
	/// <summary>
	/// Audit query based on a specific entity.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <remarks>No projection are allowed.</remarks>
	/// <seealso cref="IAuditQuery"/>
	public interface IEntityAuditQuery<T> where T: class
	{
		IList<T> ToList();
		T SingleOrDefault();

		IAuditQuery Add(IAuditCriterion criterion);

		IAuditQuery AddOrder(IAuditOrder order);

		IAuditQuery SetMaxResults(int maxResults);

		IAuditQuery SetFirstResult(int firstResult);

		IAuditQuery SetCacheable(bool cacheable);

		IAuditQuery SetCacheRegion(string cacheRegion);

		IAuditQuery SetComment(string comment);

		IAuditQuery SetFlushMode(FlushMode flushMode);

		IAuditQuery SetCacheMode(CacheMode cacheMode);

		IAuditQuery SetTimeout(int timeout);

		IAuditQuery SetLockMode(LockMode lockMode);

	}
}