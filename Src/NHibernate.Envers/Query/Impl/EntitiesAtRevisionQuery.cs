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

		public override void FillResult(IList result)
		{
			/*
			The query that should be executed in the versions table:
			SELECT e FROM ent_ver e WHERE
			  (all specified conditions, transformed, on the "e" entity) AND
			  e.revision_type != DEL AND
			  e.revision = (SELECT max(e2.revision) FROM ent_ver e2 WHERE
				e2.revision <= :revision AND e2.originalId.id = e.originalId.id)
			 */
			var maxRevQb = qb.NewSubQueryBuilder(versionsEntityName, "e2");
			var verEntCfg = verCfg.AuditEntCfg;
			var revisionPropertyPath = verEntCfg.RevisionNumberPath;
			var originalIdPropertyName = verEntCfg.OriginalIdPropName;

			// SELECT max(e2.revision)
			maxRevQb.AddProjection("max", revisionPropertyPath, false);
			// e2.revision <= :revision
			maxRevQb.RootParameters.AddWhereWithParam(revisionPropertyPath, "<=", revision);
			// e2.id = e.id
			verCfg.EntCfg[entityName].GetIdMapper().AddIdsEqualToQuery(maxRevQb.RootParameters,
																	   "e." + originalIdPropertyName,
																	   "e2." + originalIdPropertyName);

			// e.revision_type != DEL AND
			qb.RootParameters.AddWhereWithParam(verEntCfg.RevisionTypePropName, "<>", RevisionType.DEL);
			// e.revision = (SELECT max(...) ...)
			qb.RootParameters.AddWhere(revisionPropertyPath, verCfg.GlobalCfg.CorrelatedSubqueryOperator, maxRevQb);
			// all specified conditions
			foreach (var criterion in criterions)
			{
				criterion.AddToQuery(verCfg, entityName, qb, qb.RootParameters);
			}

			if (hasProjection)
			{
				BuildAndExecuteQuery(result);
				return;
			}
			var queryResult = new List<IDictionary>();
			BuildAndExecuteQuery(queryResult);
			entityInstantiator.AddInstancesFromVersionsEntities(entityName, result,
																queryResult, revision);
		}
	}
}
