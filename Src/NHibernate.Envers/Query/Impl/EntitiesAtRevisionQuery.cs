using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Query.Impl
{
	public class EntitiesAtRevisionQuery : AbstractAuditQuery 
	{
		private readonly long revision;

		public EntitiesAtRevisionQuery(AuditConfiguration verCfg,
										IAuditReaderImplementor versionsReader, 
										System.Type cls,
										long revision) : base(verCfg, versionsReader, cls)
		{
			this.revision = revision;
		}

		protected override void FillResult(IList result)
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
			var verEntCfg = VerCfg.AuditEntCfg;
			var revisionPropertyPath = verEntCfg.RevisionNumberPath;
			var originalIdPropertyName = verEntCfg.OriginalIdPropName;

			// SELECT max(e2.revision)
			maxRevQb.AddProjection("max", revisionPropertyPath, false);
			// e2.revision <= :revision
			maxRevQb.RootParameters.AddWhereWithParam(revisionPropertyPath, "<=", revision);
			// e2.id = e.id
			VerCfg.EntCfg[EntityName].IdMapper.AddIdsEqualToQuery(maxRevQb.RootParameters,
																	   "e." + originalIdPropertyName,
																	   "e2." + originalIdPropertyName);

			// e.revision_type != DEL AND
			QueryBuilder.RootParameters.AddWhereWithParam(verEntCfg.RevisionTypePropName, "<>", RevisionType.Deleted);
			// e.revision = (SELECT max(...) ...)
			QueryBuilder.RootParameters.AddWhere(revisionPropertyPath, VerCfg.GlobalCfg.CorrelatedSubqueryOperator, maxRevQb);
			// all specified conditions
			foreach (var criterion in Criterions)
			{
				criterion.AddToQuery(VerCfg, EntityName, QueryBuilder, QueryBuilder.RootParameters);
			}

			if (HasProjection)
			{
				BuildAndExecuteQuery(result);
				return;
			}
			var queryResult = new List<IDictionary>();
			BuildAndExecuteQuery(queryResult);
			EntityInstantiator.AddInstancesFromVersionsEntities(EntityName, result,
																queryResult, revision);
		}
	}
}
