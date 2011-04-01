using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Query.Criteria;
using NHibernate.Envers.Query.Order;
using NHibernate.Envers.Query.Projection;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Query.Impl
{
	public abstract class AbstractAuditQuery : IAuditQuery 
	{
		private readonly IAuditReaderImplementor _versionsReader;

		protected AbstractAuditQuery(AuditConfiguration verCfg, IAuditReaderImplementor versionsReader, System.Type cls) 
		{
			VerCfg = verCfg;
			_versionsReader = versionsReader;
			Criterions = new List<IAuditCriterion>();
			EntityInstantiator = new EntityInstantiator(verCfg, versionsReader);
			EntityName = cls.FullName;
			VersionsEntityName = verCfg.AuditEntCfg.GetAuditEntityName(EntityName);
			QueryBuilder = new QueryBuilder(VersionsEntityName, "e");
		}

		protected EntityInstantiator EntityInstantiator { get; private set; }
		protected IList<IAuditCriterion> Criterions { get; private set; }
		protected string EntityName { get; private set; }
		protected string VersionsEntityName { get; private set; }
		protected QueryBuilder QueryBuilder { get; private set; }
		protected bool HasProjection { get; private set; }
		protected bool HasOrder { get; private set; }
		protected AuditConfiguration VerCfg { get; private set; }

		protected void BuildAndExecuteQuery(IList result) 
		{
			var query = BuildQuery();
			query.List(result);
		}

		protected IQuery BuildQuery()
		{
			var query = QueryBuilder.ToQuery(_versionsReader.Session);
			SetQueryProperties(query);
			return query;
		}

		protected abstract void FillResult(IList result);

		public IList GetResultList()
		{
			var ret = new ArrayList();
			FillResult(ret);
			return ret;
		}

		public IList<T> GetResultList<T>()
		{
			var ret = new List<T>();
			FillResult(ret);
			return ret;
		}

		public T GetSingleResult<T>()
		{
			return (T) GetSingleResult();
		}

		public object GetSingleResult()
		{
			var result = new ArrayList();
			FillResult(result);

			if (result == null || result.Count == 0) 
			{
				throw new NoResultException();
			}

			if (result.Count > 1) 
			{
				throw new NonUniqueResultException(result.Count);
			}

			return result[0];
		}

		public IAuditQuery Add(IAuditCriterion criterion) 
		{
			Criterions.Add(criterion);
			return this;
		}


		public IAuditQuery AddProjection(IAuditProjection projection) 
		{
			var projectionData = projection.GetData(VerCfg);
			HasProjection = true;
			QueryBuilder.AddProjection(projectionData.First, projectionData.Second, projectionData.Third);
			return this;
		}

		public IAuditQuery AddOrder(IAuditOrder order) 
		{
			HasOrder = true;
			var orderData = order.getData(VerCfg);
			QueryBuilder.AddOrder(orderData.First, orderData.Second);
			return this;
		}

		// Query properties

		private int? maxResults;
		private int? firstResult;
		private bool? cacheable;
		private string cacheRegion;
		private string comment;
		private FlushMode flushMode;
		private CacheMode cacheMode;
		private int? timeout;
		private LockMode lockMode;

		public IAuditQuery SetMaxResults(int maxResults) 
		{
			this.maxResults = maxResults;
			return this;
		}

		public IAuditQuery SetFirstResult(int firstResult) 
		{
			this.firstResult = firstResult;
			return this;
		}

		public IAuditQuery SetCacheable(bool cacheable) 
		{
			this.cacheable = cacheable;
			return this;
		}

		public IAuditQuery SetCacheRegion(string cacheRegion) 
		{
			this.cacheRegion = cacheRegion;
			return this;
		}

		public IAuditQuery SetComment(string comment) 
		{
			this.comment = comment;
			return this;
		}

		public IAuditQuery SetFlushMode(FlushMode flushMode) 
		{
			this.flushMode = flushMode;
			return this;
		}

		public IAuditQuery SetCacheMode(CacheMode cacheMode) 
		{
			this.cacheMode = cacheMode;
			return this;
		}

		public IAuditQuery SetTimeout(int timeout) 
		{
			this.timeout = timeout;
			return this;
		}

		public IAuditQuery SetLockMode(LockMode lockMode) 
		{
			this.lockMode = lockMode;
			return this;
		}

		private void SetQueryProperties(IQuery query) 
		{
			if (maxResults != null) query.SetMaxResults((int)maxResults);
			if (firstResult != null) query.SetFirstResult((int)firstResult);
			if (cacheable != null) query.SetCacheable((bool)cacheable);
			if (cacheRegion != null) query.SetCacheRegion(cacheRegion);
			if (comment != null) query.SetComment(comment);
			query.SetFlushMode(flushMode);
			query.SetCacheMode(cacheMode);
			if (timeout != null) query.SetTimeout((int)timeout);
			if (lockMode != null) query.SetLockMode("e", lockMode);
		}
	}
}
