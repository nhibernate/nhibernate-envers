using System;
using System.Xml.Linq;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Relation;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Synchronization;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Strategy
{
	/// <summary>
	/// Default strategy is to simply persist the audit data.
	/// </summary>
	[Serializable]
	public class DefaultAuditStrategy : IAuditStrategy
	{
		private AuditConfiguration _auditConfiguration;

		public void Initialize(AuditConfiguration auditConfiguration)
		{
			_auditConfiguration = auditConfiguration;
		}

		public void Perform(ISession session, string entityName, object id, object data, object revision)
		{
			session.Save(_auditConfiguration.AuditEntCfg.GetAuditEntityName(entityName), data);
			SessionCacheCleaner.ScheduleAuditDataRemoval(session, data);
		}

		public void PerformCollectionChange(ISession session, string entityName, string propertyName, AuditConfiguration auditCfg, PersistentCollectionChangeData persistentCollectionChangeData, object revision)
		{
			var data = persistentCollectionChangeData.Data;
			session.Save(persistentCollectionChangeData.EntityName, data);
			SessionCacheCleaner.ScheduleAuditDataRemoval(session, data);
		}

		public void AddEntityAtRevisionRestriction(QueryBuilder rootQueryBuilder, Parameters parameters, string revisionProperty, string revisionEndProperty, 
																			bool addAlias, MiddleIdData idData, string revisionPropertyPath, string originalIdPropertyName,
																			string alias1, string alias2)
		{
			// create a subquery builder
			// SELECT max(e.revision) FROM versionsReferencedEntity e2
			var maxERevQb = rootQueryBuilder.NewSubQueryBuilder(idData.AuditEntityName, alias2);
			maxERevQb.AddProjection("max", revisionPropertyPath, false);
			// WHERE
			var maxERevQbParameters = maxERevQb.RootParameters;
			// e2.revision <= :revision
			maxERevQbParameters.AddWhereWithNamedParam(revisionPropertyPath, "<=", QueryConstants.RevisionParameter);
			// e2.id_ref_ed = e.id_ref_ed
			idData.OriginalMapper.AddIdsEqualToQuery(maxERevQbParameters,
					alias1 + "." + originalIdPropertyName, alias2 + "." + originalIdPropertyName);

			// add subquery to rootParameters
			var subqueryOperator = _auditConfiguration.GlobalCfg.CorrelatedSubqueryOperator;
			parameters.AddWhere(revisionProperty, addAlias, subqueryOperator, maxERevQb);
		}

		public void AddAssociationAtRevisionRestriction(QueryBuilder rootQueryBuilder,
														Parameters parameters,
														string revisionProperty,
														string revisionEndProperty,
														bool addAlias,
														MiddleIdData referencingIdData,
														string versionsMiddleEntityName,
														string eeOriginalIdPropertyPath,
														string revisionPropertyPath,
														string originalIdPropertyName,
														string alias1,
														bool inclusive,
														params MiddleComponentData[] componentDatas)
		{
			// SELECT max(ee2.revision) FROM middleEntity ee2
			var maxEeRevQb = rootQueryBuilder.NewSubQueryBuilder(versionsMiddleEntityName, QueryConstants.MiddleEntityAliasDefAudStr);
			maxEeRevQb.AddProjection("max", revisionPropertyPath, false);
			// WHERE
			var maxEeRevQbParameters = maxEeRevQb.RootParameters;
			// ee2.revision <= :revision
			maxEeRevQbParameters.AddWhereWithNamedParam(revisionPropertyPath, inclusive ? "<=" : "<", QueryConstants.RevisionParameter);
			// ee2.originalId.* = ee.originalId.*
			var ee2OriginalIdPropertyPath = QueryConstants.MiddleEntityAliasDefAudStr + "." + originalIdPropertyName;
			referencingIdData.PrefixedMapper.AddIdsEqualToQuery(maxEeRevQbParameters, eeOriginalIdPropertyPath, ee2OriginalIdPropertyPath);

			foreach (var componentData in componentDatas)
			{
				componentData.ComponentMapper.AddMiddleEqualToQuery(maxEeRevQbParameters, eeOriginalIdPropertyPath, alias1, ee2OriginalIdPropertyPath, QueryConstants.MiddleEntityAliasDefAudStr);
			}

			// add subquery to rootParameters
			parameters.AddWhere(revisionProperty, addAlias, "=", maxEeRevQb);
		}

		public void AddExtraRevisionMapping(XElement classMapping, XElement revisionInfoRelationMapping)
		{
		}
	}
}