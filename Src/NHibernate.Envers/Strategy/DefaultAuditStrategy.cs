using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Relation;
using NHibernate.Envers.Query;

namespace NHibernate.Envers.Strategy
{
	/// <summary>
	/// Default strategy is to simply persist the audit data.
	/// </summary>
	public class DefaultAuditStrategy : IAuditStrategy
	{
		public void Perform(ISession session, string entityName, AuditConfiguration auditCfg, object id, object data, object revision)
		{
			session.Save(auditCfg.AuditEntCfg.GetAuditEntityName(entityName), data);
		}

		public void PerformCollectionChange(ISession session, AuditConfiguration auditCfg, PersistentCollectionChangeData persistentCollectionChangeData, object revision)
		{
			session.Save(persistentCollectionChangeData.EntityName, persistentCollectionChangeData.Data);
		}

		public void AddEntityAtRevisionRestriction(GlobalConfiguration globalCfg, QueryBuilder rootQueryBuilder, string revisionProperty, string revisionEndProperty, bool addAlias, MiddleIdData idData, string revisionPropertyPath, string originalIdPropertyName, string alias1, string alias2)
		{
			var rootParameters = rootQueryBuilder.RootParameters;

			// create a subquery builder
			// SELECT max(e.revision) FROM versionsReferencedEntity e2
			var maxERevQb = rootQueryBuilder.NewSubQueryBuilder(idData.AuditEntityName, alias2);
			maxERevQb.AddProjection("max", revisionPropertyPath, false);
			// WHERE
			var maxERevQbParameters = maxERevQb.RootParameters;
			// e2.revision <= :revision
			maxERevQbParameters.AddWhereWithNamedParam(revisionPropertyPath, "<=", "revision");
			// e2.id_ref_ed = e.id_ref_ed
			idData.OriginalMapper.AddIdsEqualToQuery(maxERevQbParameters,
					alias1 + "." + originalIdPropertyName, alias2 + "." + originalIdPropertyName);

			// add subquery to rootParameters
			var subqueryOperator = globalCfg.CorrelatedSubqueryOperator;
			rootParameters.AddWhere(revisionProperty, addAlias, subqueryOperator, maxERevQb);
		}

		public void AddAssociationAtRevisionRestriction(QueryBuilder rootQueryBuilder,
														string revisionProperty,
														string revisionEndProperty,
														bool addAlias,
														MiddleIdData referencingIdData,
														string versionsMiddleEntityName,
														string eeOriginalIdPropertyPath,
														string revisionPropertyPath,
														string originalIdPropertyName,
														params MiddleComponentData[] componentDatas)
		{
			var rootParameters = rootQueryBuilder.RootParameters;

			// SELECT max(ee2.revision) FROM middleEntity ee2
			var maxEeRevQb = rootQueryBuilder.NewSubQueryBuilder(versionsMiddleEntityName, "ee2");
			maxEeRevQb.AddProjection("max", revisionPropertyPath, false);
			// WHERE
			var maxEeRevQbParameters = maxEeRevQb.RootParameters;
			// ee2.revision <= :revision
			maxEeRevQbParameters.AddWhereWithNamedParam(revisionPropertyPath, "<=", "revision");
			// ee2.originalId.* = ee.originalId.*
			var ee2OriginalIdPropertyPath = "ee2." + originalIdPropertyName;
			referencingIdData.PrefixedMapper.AddIdsEqualToQuery(maxEeRevQbParameters, eeOriginalIdPropertyPath, ee2OriginalIdPropertyPath);

			foreach (var componentData in componentDatas)
			{
				componentData.ComponentMapper.AddMiddleEqualToQuery(maxEeRevQbParameters, eeOriginalIdPropertyPath, ee2OriginalIdPropertyPath);
			}

			// add subquery to rootParameters
			rootParameters.AddWhere(revisionProperty, addAlias, "=", maxEeRevQb);
		}
	}
}