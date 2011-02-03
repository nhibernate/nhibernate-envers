using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;
using NHibernate.Proxy;

namespace NHibernate.Envers.Query.Impl
{
	public class RevisionsOfEntityQuery : AbstractAuditQuery 
	{
		private readonly bool selectEntitiesOnly;
		private readonly bool selectDeletedEntities;

		public RevisionsOfEntityQuery(AuditConfiguration verCfg,
									IAuditReaderImplementor versionsReader,
									System.Type cls, 
									bool selectEntitiesOnly,
									bool selectDeletedEntities) 
			:base(verCfg, versionsReader, cls)
		{
			

			this.selectEntitiesOnly = selectEntitiesOnly;
			this.selectDeletedEntities = selectDeletedEntities;
		}

		private long GetRevisionNumber(IDictionary versionsEntity) 
		{
			var verEntCfg = verCfg.AuditEntCfg;
			var originalId = verEntCfg.OriginalIdPropName;
			var revisionPropertyName = verEntCfg.RevisionFieldName;
			var revisionInfoObject = ((IDictionary) versionsEntity[originalId])[revisionPropertyName];
			var proxy = revisionInfoObject as INHibernateProxy;

			return proxy!=null ? Convert.ToInt64(proxy.HibernateLazyInitializer.Identifier) : verCfg.RevisionInfoNumberReader.RevisionNumber(revisionInfoObject);
		}

		public override void FillResult(IList result)
		{
			var verEntCfg = verCfg.AuditEntCfg;

			/*
			The query that should be executed in the versions table:
			SELECT e (unless another projection is specified) FROM ent_ver e, rev_entity r WHERE
			  e.revision_type != DEL (if selectDeletedEntities == false) AND
			  e.revision = r.revision AND
			  (all specified conditions, transformed, on the "e" entity)
			  ORDER BY e.revision ASC (unless another order or projection is specified)
			 */
			if (!selectDeletedEntities)
			{
				// e.revision_type != DEL AND
				qb.RootParameters.AddWhereWithParam(verEntCfg.RevisionTypePropName, "<>", RevisionType.DEL);
			}

			// all specified conditions, transformed
			foreach (var criterion in criterions)
			{
				criterion.AddToQuery(verCfg, entityName, qb, qb.RootParameters);
			}

			if (!hasProjection && !hasOrder)
			{
				var revisionPropertyPath = verEntCfg.RevisionNumberPath;
				qb.AddOrder(revisionPropertyPath, true);
			}

			if (!selectEntitiesOnly)
			{
				qb.AddFrom(verCfg.AuditEntCfg.RevisionInfoEntityFullClassName, "r");
				qb.RootParameters.AddWhere(verCfg.AuditEntCfg.RevisionNumberPath, true, "=", "r.id", false);
			}

			if (hasProjection)
			{
				BuildAndExecuteQuery(result);
				return;
			}
			var internalResult = new ArrayList();
			BuildAndExecuteQuery(internalResult);

			var revisionTypePropertyName = verEntCfg.RevisionTypePropName;

			foreach (var resultRow in internalResult)
			{
				IDictionary versionsEntity;
				object revisionData = null;

				if (selectEntitiesOnly)
				{
					versionsEntity = (IDictionary) resultRow;
				}
				else
				{
					var arrayResultRow = (Object[]) resultRow;
					versionsEntity = (IDictionary) arrayResultRow[0];
					revisionData = arrayResultRow[1];
				}

				var revision = GetRevisionNumber(versionsEntity);

				var entity = entityInstantiator.CreateInstanceFromVersionsEntity(entityName, versionsEntity, revision);

				result.Add(selectEntitiesOnly
								 ? entity
								 : new[] {entity, revisionData, versionsEntity[revisionTypePropertyName]});
			}
		}
	}
}
