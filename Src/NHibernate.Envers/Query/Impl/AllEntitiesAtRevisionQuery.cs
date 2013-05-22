using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Query.Impl
{
	public class AllEntitiesAtRevisionQuery<TEntity> : AbstractRevisionsQuery<TEntity> where TEntity : class
	{
		private readonly long _revision;

		public AllEntitiesAtRevisionQuery(AuditConfiguration auditConfiguration, IAuditReaderImplementor versionsReader, long revision) 
							: base(auditConfiguration, versionsReader, false, typeof(TEntity).FullName)
		{
			_revision = revision;
		}

		public override IEnumerable<TEntity> Results()
		{
			/*
			 * The query that should be executed in the versions table:
			 * SELECT e FROM ent_ver e 
			 *   WHERE
			 * (all specified conditions, transformed, on the "e" entity) AND
			 * (selecting e entities at revision :revision)
			 *   --> for DefaultAuditStrategy:
			 *     e.revision = (SELECT max(e2.revision) FROM versionsReferencedEntity e2
			 *       WHERE e2.revision <= :revision AND e2.id = e.id) 
			 *     
			 *   --> for ValidityAuditStrategy:
			 *     e.revision <= :revision and (e.endRevision > :revision or e.endRevision is null)
			 *     
			 *     AND
			 * (only non-deleted entities)
			 *     e.revision_type != DEL
			 */

			var verEntCfg = AuditConfiguration.AuditEntCfg;
			var revisionPropertyPath = verEntCfg.RevisionNumberPath;
			var originalIdPropertyName = verEntCfg.OriginalIdPropName;

			var referencedIdData = new MiddleIdData(verEntCfg, AuditConfiguration.EntCfg[EntityName].IdMappingData,
					null, EntityName, AuditConfiguration.EntCfg.IsVersioned(EntityName));

			// (selecting e entities at revision :revision)
			// --> based on auditStrategy (see above)
			AuditConfiguration.GlobalCfg.AuditStrategy.AddEntityAtRevisionRestriction(QueryBuilder, QueryBuilder.RootParameters, revisionPropertyPath,
					verEntCfg.RevisionEndFieldName, true, referencedIdData,
					revisionPropertyPath, originalIdPropertyName, QueryConstants.ReferencedEntityAlias, QueryConstants.ReferencedEntityAliasDefAudStr);
			SetIncludeDeletationClause();

			AddCriterions();

			// the result of BuildAndExecuteQuery is always the name-value pair of EntityMode.Map
			return from versionsEntity in BuildAndExecuteQuery<IDictionary>()
						 select (TEntity)EntityInstantiator.CreateInstanceFromVersionsEntity(EntityName, versionsEntity, _revision);
		}

		protected override void AddExtraParameter(IQuery query)
		{
			// add named parameter (only used for ValidAuditTimeStrategy) 
			if (query.NamedParameters.Contains(QueryConstants.RevisionParameter))
			{
				query.SetParameter(QueryConstants.RevisionParameter, _revision);
			}
		}
	}
}