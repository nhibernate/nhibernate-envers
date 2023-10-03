using System;
using System.Collections;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Reader;
using NHibernate.Proxy;
using NHibernate.SqlCommand;

namespace NHibernate.Envers.Query.Impl
{
	public partial class RevisionsOfEntityQuery : AbstractAuditQuery 
	{
		private readonly bool _selectEntitiesOnly;
		private readonly bool _selectDeletedEntities;

		public RevisionsOfEntityQuery(AuditConfiguration verCfg,
									IAuditReaderImplementor versionsReader,
									System.Type cls, 
									bool selectEntitiesOnly,
									bool selectDeletedEntities) 
			:base(verCfg, versionsReader, cls)
		{
			

			_selectEntitiesOnly = selectEntitiesOnly;
			_selectDeletedEntities = selectDeletedEntities;
		}

		public RevisionsOfEntityQuery(AuditConfiguration verCfg,
							IAuditReaderImplementor versionsReader,
							string entityName,
							bool selectEntitiesOnly,
							bool selectDeletedEntities)
			: base(verCfg, versionsReader, entityName)
		{


			_selectEntitiesOnly = selectEntitiesOnly;
			_selectDeletedEntities = selectDeletedEntities;
		}

		private long revisionNumber(IDictionary versionsEntity) 
		{
			var verEntCfg = VerCfg.AuditEntCfg;
			var originalId = verEntCfg.OriginalIdPropName;
			var revisionPropertyName = verEntCfg.RevisionFieldName;
			var revisionInfoObject = ((IDictionary) versionsEntity[originalId])[revisionPropertyName];
			var proxy = revisionInfoObject as INHibernateProxy;

			return proxy!=null ? Convert.ToInt64(proxy.HibernateLazyInitializer.Identifier) : VerCfg.RevisionInfoNumberReader.RevisionNumber(revisionInfoObject);
		}

		protected override void FillResult(IList result)
		{
			var verEntCfg = VerCfg.AuditEntCfg;

			/*
			The query that should be executed in the versions table:
			SELECT e (unless another projection is specified) FROM ent_ver e, rev_entity r WHERE
			  e.revision_type != DEL (if selectDeletedEntities == false) AND
			  e.revision = r.revision AND
			  (all specified conditions, transformed, on the "e" entity)
			  ORDER BY e.revision ASC (unless another order or projection is specified)
			 */
			if (!_selectDeletedEntities)
			{
				// e.revision_type != DEL AND
				QueryBuilder.RootParameters.AddWhereWithParam(verEntCfg.RevisionTypePropName, "<>", RevisionType.Deleted);
			}

			// all specified conditions, transformed
			foreach (var criterion in Criterions)
			{
				criterion.AddToQuery(VerCfg, VersionsReader, EntityName, QueryBuilder, QueryBuilder.RootParameters);
			}

			if (!HasProjection() && !HasOrder)
			{
				var revisionPropertyPath = verEntCfg.RevisionNumberPath;
				QueryBuilder.AddOrder(QueryConstants.ReferencedEntityAlias, revisionPropertyPath, true);
			}

			if (!_selectEntitiesOnly)
			{
				QueryBuilder.AddFrom(VerCfg.AuditEntCfg.RevisionInfoEntityFullClassName(), QueryConstants.RevisionAlias, true);
				QueryBuilder.RootParameters.AddWhere(VerCfg.AuditEntCfg.RevisionNumberPath, true, "=", QueryConstants.RevisionAlias + ".id", false);
			}

			if (HasProjection())
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

				if (_selectEntitiesOnly)
				{
					versionsEntity = (IDictionary) resultRow;
				}
				else
				{
					var arrayResultRow = (Object[]) resultRow;
					versionsEntity = (IDictionary) arrayResultRow[0];
					revisionData = arrayResultRow[1];
				}

				var revision = revisionNumber(versionsEntity);

				var entity = EntityInstantiator.CreateInstanceFromVersionsEntity(EntityName, versionsEntity, revision);

				result.Add(_selectEntitiesOnly
								 ? entity
								 : new[] {entity, revisionData, versionsEntity[revisionTypePropertyName]});
			}
		}

		public override IAuditQuery TraverseRelation(string associationName, JoinType joinType)
		{
			throw new NotSupportedException("Not yet implemented for revision of entity queries");
		}
	}
}
