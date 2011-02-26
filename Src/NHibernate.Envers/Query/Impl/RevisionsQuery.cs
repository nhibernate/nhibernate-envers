using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Query.Criteria;
using NHibernate.Envers.Query.Order;
using NHibernate.Envers.Reader;
using NHibernate.Proxy;

namespace NHibernate.Envers.Query.Impl
{
	public class RevisionsQuery<TEntity> : IEntityAuditQuery<TEntity> where TEntity : class
	{
		private readonly bool includesDeletations;
		private CacheMode cacheMode;
		private string cacheRegion;
		private bool? cacheable;
		private string comment;
		private int? firstResult;
		private FlushMode flushMode;
		private LockMode lockMode;
		private int? maxResults;
		private int? timeout;
		private readonly AuditConfiguration auditConfiguration;
		private readonly IAuditReaderImplementor versionsReader;
		private readonly List<IAuditCriterion> criterions;
		private readonly EntityInstantiator entityInstantiator;
		private readonly string entityName;
		private readonly string versionsEntityName;
		private readonly QueryBuilder queryBuilder;
		private bool hasOrder;

		public RevisionsQuery(AuditConfiguration auditConfiguration,
							IAuditReaderImplementor versionsReader,
							bool includesDeletations)
		{
			this.auditConfiguration = auditConfiguration;
			this.versionsReader = versionsReader;

			criterions = new List<IAuditCriterion>();
			entityInstantiator = new EntityInstantiator(auditConfiguration, versionsReader);

			entityName = typeof(TEntity).FullName;

			versionsEntityName = auditConfiguration.AuditEntCfg.GetAuditEntityName(entityName);

			queryBuilder = new QueryBuilder(versionsEntityName, "e");
			this.includesDeletations = includesDeletations;
		}

		#region IEntityAuditQuery<TEntity> Members

		public IEnumerable<TEntity> Results()
		{
			var verEntCfg = auditConfiguration.AuditEntCfg;

			/*
			The query that should be executed in the versions table:
			SELECT e FROM ent_ver e, rev_entity r WHERE
			  e.revision_type != DEL (if includesDeletations == false) AND
			  e.revision = r.revision AND
			  (all specified conditions, transformed, on the "e" entity)
			  ORDER BY e.revision ASC (unless another order is specified)
			 */
			if (!includesDeletations)
			{
				// e.revision_type != DEL AND
				queryBuilder.RootParameters.AddWhereWithParam(verEntCfg.RevisionTypePropName, "<>", RevisionType.Deleted);
			}

			// all specified conditions, transformed
			foreach (var criterion in criterions)
			{
				criterion.AddToQuery(auditConfiguration, entityName, queryBuilder, queryBuilder.RootParameters);
			}

			if (!hasOrder)
			{
				var revisionPropertyPath = verEntCfg.RevisionNumberPath;
				queryBuilder.AddOrder(revisionPropertyPath, true);
			}

			// the result of BuildAndExecuteQuery is always the name-value pair of EntityMode.Map
			return from versionsEntity in BuildAndExecuteQuery<IDictionary>()
						 let revision = GetRevisionNumberFromDynamicEntity(versionsEntity)
						 select (TEntity)entityInstantiator.CreateInstanceFromVersionsEntity(entityName, versionsEntity, revision);
		}

		protected IList<TExpectedResult> BuildAndExecuteQuery<TExpectedResult>()
		{
			var querySb = new StringBuilder();
			var queryParamValues = new Dictionary<string, object>();

			queryBuilder.Build(querySb, queryParamValues);

			var query = versionsReader.Session.CreateQuery(querySb.ToString());
			foreach (var paramValue in queryParamValues)
			{
				query.SetParameter(paramValue.Key, paramValue.Value);
			}

			SetQueryProperties(query);
			return query.List<TExpectedResult>();
		}

		private long GetRevisionNumberFromDynamicEntity(IDictionary versionsEntity)
		{
			var verEntCfg = auditConfiguration.AuditEntCfg;
			var originalId = verEntCfg.OriginalIdPropName;
			var revisionPropertyName = verEntCfg.RevisionFieldName;
			var revisionInfoObject = ((IDictionary)versionsEntity[originalId])[revisionPropertyName];
			var proxy = revisionInfoObject as INHibernateProxy; // TODO: usage of proxyfactory IsProxy

			return proxy != null ? Convert.ToInt64(proxy.HibernateLazyInitializer.Identifier) : auditConfiguration.RevisionInfoNumberReader.RevisionNumber(revisionInfoObject);
		}

		public TEntity Single()
		{
			var results = Results();
			using (var enumerator = results.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw new NoResultException();
				}
				TEntity current = enumerator.Current;
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
			var orderData = order.getData(auditConfiguration);
			queryBuilder.AddOrder(orderData.First, orderData.Second);
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

		private void SetQueryProperties(IQuery query)
		{
			if (maxResults != null)
			{
				query.SetMaxResults((int) maxResults);
			}
			if (firstResult != null)
			{
				query.SetFirstResult((int) firstResult);
			}
			if (cacheable != null)
			{
				query.SetCacheable((bool) cacheable);
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
			if (timeout != null)
			{
				query.SetTimeout((int) timeout);
			}
			if (lockMode != null)
			{
				query.SetLockMode("e", lockMode);
			}
		}
	}
}