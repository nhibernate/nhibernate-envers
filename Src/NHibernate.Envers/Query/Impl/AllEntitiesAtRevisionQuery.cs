using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Query.Impl
{
	public class AllEntitiesAtRevisionQuery<TEntity> : AbstractRevisionsQuery<TEntity> where TEntity : class
	{
		private readonly long revision;

		public AllEntitiesAtRevisionQuery(AuditConfiguration auditConfiguration, IAuditReaderImplementor versionsReader, long revision) 
							: base(auditConfiguration, versionsReader, false, typeof(TEntity).FullName)
		{
			this.revision = revision;
		}

		public override IEnumerable<TEntity> Results()
		{
			/*
			The query that should be executed in the versions table:
			SELECT e FROM ent_ver e WHERE
			  (all specified conditions, transformed, on the "e" entity) AND
			  e.revision_type != DEL AND
			  e.revision = (SELECT max(e2.revision) FROM ent_ver e2 WHERE
				e2.revision <= :revision AND e2.originalId.id = e.originalId.id)
			 */
			var maxRevQb = QueryBuilder.NewSubQueryBuilder(VersionsEntityName, "e2");
			var verEntCfg = AuditConfiguration.AuditEntCfg;
			var revisionPropertyPath = verEntCfg.RevisionNumberPath;
			var originalIdPropertyName = verEntCfg.OriginalIdPropName;

			// SELECT max(e2.revision)
			maxRevQb.AddProjection("max", revisionPropertyPath, false);
			// e2.revision <= :revision
			maxRevQb.RootParameters.AddWhereWithParam(revisionPropertyPath, "<=", revision);
			// e2.id = e.id
			AuditConfiguration.EntCfg[EntityName].IdMapper.AddIdsEqualToQuery(maxRevQb.RootParameters,
																		 QueryConstants.ReferencedEntityAlias + "." + originalIdPropertyName,
																		 "e2." + originalIdPropertyName);
			SetIncludeDeletationClause();
			
			// e.revision = (SELECT max(...) ...)
			QueryBuilder.RootParameters.AddWhere(revisionPropertyPath, AuditConfiguration.GlobalCfg.CorrelatedSubqueryOperator, maxRevQb);

			AddCriterions();

			// the result of BuildAndExecuteQuery is always the name-value pair of EntityMode.Map
			return from versionsEntity in BuildAndExecuteQuery<IDictionary>()
						 select (TEntity)EntityInstantiator.CreateInstanceFromVersionsEntity(EntityName, versionsEntity, revision);
		}
	}
}