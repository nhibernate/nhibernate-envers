using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Query.Criteria;
using NHibernate.Envers.Query.Order;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools.Query;
using NHibernate.Proxy;

namespace NHibernate.Envers.Query.Impl
{
	public abstract class AbstractRevisionsQuery<TEntity> : IEntityAuditQuery<TEntity> where TEntity : class
	{
		private readonly string versionsEntityName;
		private readonly IAuditReaderImplementor versionsReader;
		private readonly AuditConfiguration auditConfiguration;
		private CacheMode cacheMode;
		private string cacheRegion;
		private bool? cacheable;
		private string comment;
		private readonly List<IAuditCriterion> criterions;
		private readonly EntityInstantiator entityInstantiator;
		private readonly string entityName;
		private int? firstResult;
		private FlushMode flushMode;
		private bool hasOrder;
		private readonly bool includesDeletations;
		private LockMode lockMode;
		private int? maxResults;
		private readonly QueryBuilder queryBuilder;
		private int? timeout;

		protected AbstractRevisionsQuery(AuditConfiguration auditConfiguration,
		                              IAuditReaderImplementor versionsReader,
		                              bool includesDeletations, string entityName)
		{
			this.auditConfiguration = auditConfiguration;
			this.versionsReader = versionsReader;

			criterions = new List<IAuditCriterion>();
			entityInstantiator = new EntityInstantiator(auditConfiguration, versionsReader);

			this.entityName = entityName;

			versionsEntityName = auditConfiguration.AuditEntCfg.GetAuditEntityName(this.entityName);

			queryBuilder = new QueryBuilder(versionsEntityName, QueryConstants.ReferencedEntityAlias);
			this.includesDeletations = includesDeletations;

			if (!auditConfiguration.EntCfg.IsVersioned(EntityName))
			{
				throw new NotAuditedException(EntityName, EntityName + " is not versioned!");
			}
		}

		public EntityInstantiator EntityInstantiator
		{
			get { return entityInstantiator; }
		}

		public AuditConfiguration AuditConfiguration
		{
			get { return auditConfiguration; }
		}

		public QueryBuilder QueryBuilder
		{
			get { return queryBuilder; }
		}

		public string EntityName
		{
			get { return entityName; }
		}

		public string VersionsEntityName
		{
			get { return versionsEntityName; }
		}

		#region IEntityAuditQuery<TEntity> Members

		public abstract IEnumerable<TEntity> Results();

		public TEntity Single()
		{
			var results = Results();
			using (var enumerator = results.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					return null;
				}
				var current = enumerator.Current;
				if (!enumerator.MoveNext())
				{
					return current;
				}
			}

			throw new NonUniqueResultException(2); // TODO : it need a modification in NH: the exception should work even without the count of results
		}

		public IEntityAuditQuery<TEntity> Add(IAuditCriterion criterion)
		{
			criterions.Add(criterion);
			return this;
		}

		public IEntityAuditQuery<TEntity> AddOrder(IAuditOrder order)
		{
			hasOrder = true;
			var orderData = order.GetData(auditConfiguration);
			var propertyName = CriteriaTools.DeterminePropertyName(AuditConfiguration, versionsReader, entityName, orderData.Item1);
			queryBuilder.AddOrder(propertyName, orderData.Item2);
			return this;
		}

		public IEntityAuditQuery<TEntity> SetMaxResults(int maxResults)
		{
			this.maxResults = maxResults;
			return this;
		}

		public IEntityAuditQuery<TEntity> SetFirstResult(int firstResult)
		{
			this.firstResult = firstResult;
			return this;
		}

		public IEntityAuditQuery<TEntity> SetCacheable(bool cacheable)
		{
			this.cacheable = cacheable;
			return this;
		}

		public IEntityAuditQuery<TEntity> SetCacheRegion(string cacheRegion)
		{
			this.cacheRegion = cacheRegion;
			return this;
		}

		public IEntityAuditQuery<TEntity> SetComment(string comment)
		{
			this.comment = comment;
			return this;
		}

		public IEntityAuditQuery<TEntity> SetFlushMode(FlushMode flushMode)
		{
			this.flushMode = flushMode;
			return this;
		}

		public IEntityAuditQuery<TEntity> SetCacheMode(CacheMode cacheMode)
		{
			this.cacheMode = cacheMode;
			return this;
		}

		public IEntityAuditQuery<TEntity> SetTimeout(int timeout)
		{
			this.timeout = timeout;
			return this;
		}

		public IEntityAuditQuery<TEntity> SetLockMode(LockMode lockMode)
		{
			this.lockMode = lockMode;
			return this;
		}

		#endregion

		protected void AddOrders()
		{
			if (hasOrder)
			{
				return;
			}
			var revisionPropertyPath = auditConfiguration.AuditEntCfg.RevisionNumberPath;
			queryBuilder.AddOrder(revisionPropertyPath, true);
		}

		protected void AddCriterions()
		{
			// all specified conditions, transformed
			foreach (var criterion in criterions)
			{
				criterion.AddToQuery(auditConfiguration, versionsReader, entityName, queryBuilder, queryBuilder.RootParameters);
			}
		}

		protected void SetIncludeDeletationClause()
		{
			if (!includesDeletations)
			{
				// e.revision_type != DEL AND
				queryBuilder.RootParameters.AddWhereWithParam(auditConfiguration.AuditEntCfg.RevisionTypePropName, "<>", RevisionType.Deleted);
			}
		}

		protected long GetRevisionNumberFromDynamicEntity(IDictionary versionsEntity)
		{
			var auditEntitiesConfiguration = AuditConfiguration.AuditEntCfg;
			var originalId = auditEntitiesConfiguration.OriginalIdPropName;
			var revisionPropertyName = auditEntitiesConfiguration.RevisionFieldName;
			var revisionInfoObject = ((IDictionary)versionsEntity[originalId])[revisionPropertyName];
			var proxy = revisionInfoObject as INHibernateProxy; 

			return proxy == null ? 
				AuditConfiguration.RevisionInfoNumberReader.RevisionNumber(revisionInfoObject) : 
				Convert.ToInt64(proxy.HibernateLazyInitializer.Identifier);
		}

		protected IList<TResult> BuildAndExecuteQuery<TResult>()
		{
			var querySb = new StringBuilder();
			var queryParamValues = new Dictionary<string, object>();

			queryBuilder.Build(querySb, queryParamValues);

			var query = versionsReader.Session.CreateQuery(querySb.ToString());
			foreach (var paramValue in queryParamValues)
			{
				query.SetParameter(paramValue.Key, paramValue.Value);
			}
			AddExtraParameter(query);
			SetQueryProperties(query);
			return query.List<TResult>();
		}

		protected virtual void AddExtraParameter(IQuery query)
		{
		}

		private void SetQueryProperties(IQuery query)
		{
			if (maxResults.HasValue)
			{
				query.SetMaxResults(maxResults.Value);
			}
			if (firstResult.HasValue)
			{
				query.SetFirstResult(firstResult.Value);
			}
			if (cacheable.HasValue)
			{
				query.SetCacheable(cacheable.Value);
			}
			if (cacheRegion != null)
			{
				query.SetCacheRegion(cacheRegion);
			}
			if (comment != null)
			{
				query.SetComment(comment);
			}
			query.SetFlushMode(flushMode);
			query.SetCacheMode(cacheMode);
			if (timeout.HasValue)
			{
				query.SetTimeout(timeout.Value);
			}
			if (lockMode != null)
			{
				query.SetLockMode(QueryConstants.ReferencedEntityAlias, lockMode);
			}
		}
	}
}