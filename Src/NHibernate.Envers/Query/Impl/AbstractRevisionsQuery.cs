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
using NHibernate.SqlCommand;

namespace NHibernate.Envers.Query.Impl
{
	public abstract partial class AbstractRevisionsQuery<TEntity> : IEntityAuditQuery<TEntity> where TEntity : class
	{
		private readonly IAuditReaderImplementor _versionsReader;
		private CacheMode _cacheMode;
		private string _cacheRegion;
		private bool? _cacheable;
		private string _comment;
		private readonly List<IAuditCriterion> _criterions;
		private int? _firstResult;
		private FlushMode _flushMode;
		private bool _hasOrder;
		private readonly bool _includesDeletations;
		private LockMode _lockMode;
		private int? _maxResults;
		private int? _timeout;
		private readonly IList<AuditRevisionsAssociationQuery<TEntity>> _associationQueries;
		private readonly IDictionary<string, AuditRevisionsAssociationQuery<TEntity>> _associationQueryMap;

		protected AbstractRevisionsQuery(AuditConfiguration auditConfiguration,
		                              IAuditReaderImplementor versionsReader,
		                              bool includesDeletations, 
																	string entityName)
		{
			AuditConfiguration = auditConfiguration;
			_versionsReader = versionsReader;

			_criterions = new List<IAuditCriterion>();
			EntityInstantiator = new EntityInstantiator(auditConfiguration, versionsReader);

			EntityName = entityName;

			VersionsEntityName = auditConfiguration.AuditEntCfg.GetAuditEntityName(EntityName);

			QueryBuilder = new QueryBuilder(VersionsEntityName, QueryConstants.ReferencedEntityAlias);
			_includesDeletations = includesDeletations;

			if (!auditConfiguration.EntCfg.IsVersioned(EntityName))
			{
				throw new NotAuditedException(EntityName, EntityName + " is not versioned!");
			}

			_associationQueries = new List<AuditRevisionsAssociationQuery<TEntity>>();
			_associationQueryMap = new Dictionary<string, AuditRevisionsAssociationQuery<TEntity>>();
		}

		protected EntityInstantiator EntityInstantiator { get; }
		protected AuditConfiguration AuditConfiguration { get; }
		protected QueryBuilder QueryBuilder { get; }
		protected string EntityName { get; }
		protected string VersionsEntityName { get; }

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
			_criterions.Add(criterion);
			return this;
		}

		public IEntityAuditQuery<TEntity> AddOrder(IAuditOrder order)
		{
			_hasOrder = true;
			var orderData = order.GetData(AuditConfiguration);
			var propertyName = CriteriaTools.DeterminePropertyName(AuditConfiguration, _versionsReader, EntityName, orderData.Item1);
			QueryBuilder.AddOrder(QueryConstants.ReferencedEntityAlias, propertyName, orderData.Item2);
			return this;
		}

		public IEntityAuditQuery<TEntity> SetMaxResults(int maxResults)
		{
			_maxResults = maxResults;
			return this;
		}

		public IEntityAuditQuery<TEntity> SetFirstResult(int firstResult)
		{
			_firstResult = firstResult;
			return this;
		}

		public IEntityAuditQuery<TEntity> SetCacheable(bool cacheable)
		{
			_cacheable = cacheable;
			return this;
		}

		public IEntityAuditQuery<TEntity> SetCacheRegion(string cacheRegion)
		{
			_cacheRegion = cacheRegion;
			return this;
		}

		public IEntityAuditQuery<TEntity> SetComment(string comment)
		{
			_comment = comment;
			return this;
		}

		public IEntityAuditQuery<TEntity> SetFlushMode(FlushMode flushMode)
		{
			_flushMode = flushMode;
			return this;
		}

		public IEntityAuditQuery<TEntity> SetCacheMode(CacheMode cacheMode)
		{
			_cacheMode = cacheMode;
			return this;
		}

		public IEntityAuditQuery<TEntity> SetTimeout(int timeout)
		{
			_timeout = timeout;
			return this;
		}

		public IEntityAuditQuery<TEntity> SetLockMode(LockMode lockMode)
		{
			_lockMode = lockMode;
			return this;
		}

		public IEntityAuditQuery<TEntity> TraverseRelation(string associationName, JoinType joinType)
		{
			AuditRevisionsAssociationQuery<TEntity> result;
			if (!_associationQueryMap.TryGetValue(associationName, out result))
			{
				result = new AuditRevisionsAssociationQuery<TEntity>(AuditConfiguration, _versionsReader, this, QueryBuilder, EntityName, associationName, joinType, QueryConstants.ReferencedEntityAlias);
				_associationQueries.Add(result);
				_associationQueryMap[associationName] = result;
			}
			return result;
		}

		public IEntityAuditQuery<TEntity> Up()
		{
			return this;
		}

		protected void AddOrders()
		{
			if (_hasOrder)
			{
				return;
			}
			var revisionPropertyPath = AuditConfiguration.AuditEntCfg.RevisionNumberPath;
			QueryBuilder.AddOrder(QueryConstants.ReferencedEntityAlias, revisionPropertyPath, true);
		}

		protected void AddCriterions()
		{
			// all specified conditions, transformed
			foreach (var criterion in _criterions)
			{
				criterion.AddToQuery(AuditConfiguration, _versionsReader, EntityName, QueryBuilder, QueryBuilder.RootParameters);
			}

			foreach (var associationQuery in _associationQueries)
			{
				associationQuery.AddCriterionsToQuery(_versionsReader);
			}
		}

		protected void SetIncludeDeletationClause()
		{
			if (!_includesDeletations)
			{
				// e.revision_type != DEL AND
				QueryBuilder.RootParameters.AddWhereWithParam(AuditConfiguration.AuditEntCfg.RevisionTypePropName, "<>", RevisionType.Deleted);
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

			QueryBuilder.Build(querySb, queryParamValues);

			var query = _versionsReader.Session.CreateQuery(querySb.ToString());
			foreach (var paramValue in queryParamValues)
			{
				query.SetParameter(paramValue.Key, paramValue.Value);
			}
			AddExtraParameter(query);
			setQueryProperties(query);


			return query.List<TResult>();
		}

		protected virtual void AddExtraParameter(IQuery query)
		{
		}

		private void setQueryProperties(IQuery query)
		{
			if (_maxResults.HasValue)
			{
				query.SetMaxResults(_maxResults.Value);
			}
			if (_firstResult.HasValue)
			{
				query.SetFirstResult(_firstResult.Value);
			}
			if (_cacheable.HasValue)
			{
				query.SetCacheable(_cacheable.Value);
			}
			if (_cacheRegion != null)
			{
				query.SetCacheRegion(_cacheRegion);
			}
			if (_comment != null)
			{
				query.SetComment(_comment);
			}
			query.SetFlushMode(_flushMode);
			query.SetCacheMode(_cacheMode);
			if (_timeout.HasValue)
			{
				query.SetTimeout(_timeout.Value);
			}
			if (_lockMode != null)
			{
				query.SetLockMode(QueryConstants.ReferencedEntityAlias, _lockMode);
			}
		}
	}
}