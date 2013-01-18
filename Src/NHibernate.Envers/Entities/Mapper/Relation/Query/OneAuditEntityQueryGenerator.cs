using System;
using System.Text;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Strategy;
using NHibernate.Envers.Tools.Query;
using NHibernate.Transform;

namespace NHibernate.Envers.Entities.Mapper.Relation.Query
{
	/// <summary>
	/// Selects data from an audit entity.
	/// </summary>
	[Serializable]
	public sealed class OneAuditEntityQueryGenerator: IRelationQueryGenerator 
	{
		private readonly string _queryString;
		private readonly MiddleIdData _referencingIdData;

		public OneAuditEntityQueryGenerator(AuditEntitiesConfiguration verEntCfg,
											IAuditStrategy auditStrategy,
											MiddleIdData referencingIdData, 
											string referencedEntityName,
											MiddleIdData referencedIdData) 
		{
			_referencingIdData = referencingIdData;

			/*
			 * The query that we need to create:
			 *   SELECT new list(e) FROM versionsReferencedEntity e
			 *   WHERE
			 * (only entities referenced by the association; id_ref_ing = id of the referencing entity)
			 *     e.id_ref_ing = :id_ref_ing AND
			 * (selecting e entities at revision :revision)
			 *		--> for DefaultAuditStrategy:
			 *			e.revision = (SELECT max(e2.revision) FROM versionsReferencedEntity e2
			 *			WHERE e2.revision <= :revision AND e2.id = e.id)
			 *		--> for ValidityAuditStrategy
			 *			e.revision <= :revision and (e.endRevision > :revision or e.endRevision is null)
			 *	AND
			 * (only non-deleted entities)
			 *     e.revision_type != DEL
			 */
			var revisionPropertyPath = verEntCfg.RevisionNumberPath;
			var originalIdPropertyName = verEntCfg.OriginalIdPropName;
			var versionsReferencedEntityName = verEntCfg.GetAuditEntityName(referencedEntityName);

			// SELECT new list(e) FROM versionsEntity e
			var qb = new QueryBuilder(versionsReferencedEntityName, QueryConstants.ReferencedEntityAlias);
			//qb.AddProjection("new list", "e", false, false);
			// WHERE
			var rootParameters = qb.RootParameters;
			// e.id_ref_ed = :id_ref_ed
			referencingIdData.PrefixedMapper.AddNamedIdEqualsToQuery(rootParameters, null, true);


			// (selecting e entities at revision :revision)
			// --> based on auditStrategy (see above)
			auditStrategy.AddEntityAtRevisionRestriction(qb, revisionPropertyPath, verEntCfg.RevisionEndFieldName, true,
				referencedIdData, revisionPropertyPath, originalIdPropertyName, QueryConstants.ReferencedEntityAlias, QueryConstants.ReferencedEntityAliasDefAudStr);


			// e.revision_type != DEL
			rootParameters.AddWhereWithNamedParam(verEntCfg.RevisionTypePropName, false, "!=", QueryConstants.DelRevisionTypeParameter);

			var sb = new StringBuilder();
			qb.Build(sb, null);
			_queryString = sb.ToString();
		}

		public IQuery GetQuery(IAuditReaderImplementor versionsReader, object primaryKey, long revision) 
		{
			var query = versionsReader.Session.CreateQuery(_queryString);
			query.SetParameter(QueryConstants.RevisionParameter, revision);
			query.SetParameter(QueryConstants.DelRevisionTypeParameter, RevisionType.Deleted);
			foreach (var paramData in _referencingIdData.PrefixedMapper.MapToQueryParametersFromId(primaryKey)) 
			{
				paramData.SetParameterValue(query);
			}
			query.SetResultTransformer(Transformers.ToList);
			return query;
		}
	}
}