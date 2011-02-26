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
		IEnumerable<T> Results();
		T Single();

		IEntityAuditQuery<T> Add(IAuditCriterion criterion);

		IEntityAuditQuery<T> AddOrder(IAuditOrder order);

		IEntityAuditQuery<T> SetMaxResults(int maxResults);

		IEntityAuditQuery<T> SetFirstResult(int firstResult);

		IEntityAuditQuery<T> SetCacheable(bool cacheable);

		IEntityAuditQuery<T> SetCacheRegion(string cacheRegion);

		IEntityAuditQuery<T> SetComment(string comment);

		IEntityAuditQuery<T> SetFlushMode(FlushMode flushMode);

		IEntityAuditQuery<T> SetCacheMode(CacheMode cacheMode);

		IEntityAuditQuery<T> SetTimeout(int timeout);

		IEntityAuditQuery<T> SetLockMode(LockMode lockMode);
	}
}