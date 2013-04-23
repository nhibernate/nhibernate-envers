using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Query.Criteria;
using NHibernate.Envers.Query.Order;
using NHibernate.Envers.Query.Projection;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Impl
{
	public abstract class AbstractAuditQuery : IAuditQuery 
	{
		protected AbstractAuditQuery(AuditConfiguration verCfg, IAuditReaderImplementor versionsReader, System.Type cls)
				: this(verCfg, versionsReader, cls.FullName)
		{
		}

		protected AbstractAuditQuery(AuditConfiguration verCfg, IAuditReaderImplementor versionsReader, string entityName)
		{
			VerCfg = verCfg;
			EntityName = entityName;
			VersionsReader = versionsReader;
			Criterions = new List<IAuditCriterion>();
			EntityInstantiator = new EntityInstantiator(verCfg, versionsReader);
			EntityName = entityName;
			VersionsEntityName = verCfg.AuditEntCfg.GetAuditEntityName(EntityName);
			QueryBuilder = new QueryBuilder(VersionsEntityName, QueryConstants.ReferencedEntityAlias);

			if (!verCfg.EntCfg.IsVersioned(EntityName))
			{
				throw new NotAuditedException(EntityName, EntityName + " is not versioned!");
			}
		}

		protected IAuditReaderImplementor VersionsReader { get; private set; }
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
			var query = QueryBuilder.ToQuery(VersionsReader.Session);
			setQueryProperties(query);
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
			switch (result.Count)
			{
				case 0:
					return null;
				case 1:
					return result[0];
				default:
					throw new NonUniqueResultException(result.Count);
			}
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
			var propertyName = CriteriaTools.DeterminePropertyName(VerCfg, VersionsReader, EntityName, projectionData.Item2);
			QueryBuilder.AddProjection(projectionData.Item1, propertyName, projectionData.Item3);
			return this;
		}

		public IAuditQuery AddOrder(IAuditOrder order) 
		{
			HasOrder = true;
			var orderData = order.GetData(VerCfg);
			var propertyName = CriteriaTools.DeterminePropertyName(VerCfg, VersionsReader, EntityName, orderData.Item1);
			QueryBuilder.AddOrder(propertyName, orderData.Item2);
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

		private void setQueryProperties(IQuery query) 
		{
			if (maxResults != null) query.SetMaxResults((int)maxResults);
			if (firstResult != null) query.SetFirstResult((int)firstResult);
			if (cacheable != null) query.SetCacheable((bool)cacheable);
			if (cacheRegion != null) query.SetCacheRegion(cacheRegion);
			if (comment != null) query.SetComment(comment);
			query.SetFlushMode(flushMode);
			query.SetCacheMode(cacheMode);
			if (timeout != null) query.SetTimeout((int)timeout);
			if (lockMode != null) query.SetLockMode(QueryConstants.ReferencedEntityAlias, lockMode);
		}
	}
}
