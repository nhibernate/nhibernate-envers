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
using System;
using System.Linq;
using NHibernate.SqlCommand;

namespace NHibernate.Envers.Query.Impl
{
	public abstract partial class AbstractAuditQuery : IAuditQueryImplementor
	{
		private int? _maxResults;
		private int? _firstResult;
		private bool? _cacheable;
		private string _cacheRegion;
		private string _comment;
		private FlushMode _flushMode;
		private CacheMode _cacheMode;
		private int? _timeout;
		private LockMode _lockMode;

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
			AssociationQueries=new List<AuditAssociationQuery>();
			associationQueryMap = new Dictionary<string, AuditAssociationQuery>();
			projections = new List<Tuple<string, IAuditProjection>>();
		}

		protected IAuditReaderImplementor VersionsReader { get; }
		protected EntityInstantiator EntityInstantiator { get; }
		protected IList<IAuditCriterion> Criterions { get; }
		protected string EntityName { get; }
		protected string VersionsEntityName { get; }
		protected QueryBuilder QueryBuilder { get; }
		protected bool HasOrder { get; private set; }
		protected AuditConfiguration VerCfg { get; }
		protected IList<AuditAssociationQuery> AssociationQueries { get; }
		private IDictionary<string, AuditAssociationQuery> associationQueryMap { get; }
		private IList<Tuple<string, IAuditProjection>> projections { get; }

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
			RegisterProjection(EntityName, projection);
			var propertyName = CriteriaTools.DeterminePropertyName(VerCfg, VersionsReader, EntityName, projectionData.Item2);
			QueryBuilder.AddProjection(projectionData.Item1, QueryConstants.ReferencedEntityAlias, propertyName, projectionData.Item3);
			return this;
		}

		public void RegisterProjection(string entityName, IAuditProjection projection)
		{
			projections.Add(new Tuple<string, IAuditProjection>(entityName, projection));
		}

		protected bool HasProjection()
		{
			return projections.Any();
		}

		public IAuditQuery AddOrder(IAuditOrder order) 
		{
			HasOrder = true;
			var orderData = order.GetData(VerCfg);
			var propertyName = CriteriaTools.DeterminePropertyName(VerCfg, VersionsReader, EntityName, orderData.Item1);
			QueryBuilder.AddOrder(QueryConstants.ReferencedEntityAlias, propertyName, orderData.Item2);
			return this;
		}


		public IAuditQuery SetMaxResults(int maxResults) 
		{
			_maxResults = maxResults;
			return this;
		}

		public IAuditQuery SetFirstResult(int firstResult) 
		{
			_firstResult = firstResult;
			return this;
		}

		public IAuditQuery SetCacheable(bool cacheable) 
		{
			_cacheable = cacheable;
			return this;
		}

		public IAuditQuery SetCacheRegion(string cacheRegion) 
		{
			_cacheRegion = cacheRegion;
			return this;
		}

		public IAuditQuery SetComment(string comment) 
		{
			_comment = comment;
			return this;
		}

		public IAuditQuery SetFlushMode(FlushMode flushMode) 
		{
			_flushMode = flushMode;
			return this;
		}

		public IAuditQuery SetCacheMode(CacheMode cacheMode) 
		{
			_cacheMode = cacheMode;
			return this;
		}

		public IAuditQuery SetTimeout(int timeout) 
		{
			_timeout = timeout;
			return this;
		}

		public IAuditQuery SetLockMode(LockMode lockMode) 
		{
			_lockMode = lockMode;
			return this;
		}

		public virtual IAuditQuery TraverseRelation(string associationName, JoinType joinType)
		{
			AuditAssociationQuery result;
			if (!associationQueryMap.TryGetValue(associationName, out result))
			{
				result = new AuditAssociationQuery(VerCfg, VersionsReader, this, QueryBuilder, EntityName, associationName, joinType, QueryConstants.ReferencedEntityAlias);
				AssociationQueries.Add(result);
				associationQueryMap[associationName] = result;
			}
			return result;
		}

		public IAuditQuery Up()
		{
			return this;
		}

		private void setQueryProperties(IQuery query) 
		{
			if (_maxResults != null) query.SetMaxResults((int)_maxResults);
			if (_firstResult != null) query.SetFirstResult((int)_firstResult);
			if (_cacheable != null) query.SetCacheable((bool)_cacheable);
			if (_cacheRegion != null) query.SetCacheRegion(_cacheRegion);
			if (_comment != null) query.SetComment(_comment);
			query.SetFlushMode(_flushMode);
			query.SetCacheMode(_cacheMode);
			if (_timeout != null) query.SetTimeout((int)_timeout);
			if (_lockMode != null) query.SetLockMode(QueryConstants.ReferencedEntityAlias, _lockMode);
		}

		protected void ApplyProjections(IQuery query, IList resultToFill, long revision)
		{
			if (HasProjection())
			{
				foreach (var qr in query.List())
				{
					if (projections.Count == 1)
					{
						// qr is the value of the projection itself
						var projection = projections[0];
						resultToFill.Add(projection.Item2.ConvertQueryResult(VerCfg, EntityInstantiator, projection.Item1, revision, qr));
					}
					else
					{
						// qr is an array where each of its components holds the value of corresponding projection
						var qresults = (object[]) qr;
						var tresults = new object[qresults.Length];
						for (var i = 0; i < qresults.Length; i++)
						{
							var projection = projections[i];
							tresults[i] = projection.Item2.ConvertQueryResult(VerCfg, EntityInstantiator, projection.Item1, revision, qresults[i]);
						}
						resultToFill.Add(tresults);
					}
				}
			}
			else
			{
				var queryResult = new List<IDictionary>();
				query.List(queryResult);
				EntityInstantiator.AddInstancesFromVersionsEntities(EntityName, resultToFill, queryResult, revision);
			}
		}
	}
}
