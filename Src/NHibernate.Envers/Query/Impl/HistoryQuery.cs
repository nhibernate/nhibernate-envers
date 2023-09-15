using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Query.Impl
{
	public partial class HistoryQuery<TEntity, TRevisionEntity> : AbstractRevisionsQuery<IRevisionEntityInfo<TEntity, TRevisionEntity>>
		where TEntity : class
		where TRevisionEntity : class
	{
		public HistoryQuery(AuditConfiguration auditConfiguration, IAuditReaderImplementor versionsReader, bool includesDeletations)
			: base(auditConfiguration, versionsReader, includesDeletations, typeof (TEntity).FullName) {}

		public override IEnumerable<IRevisionEntityInfo<TEntity, TRevisionEntity>> Results()
		{
			var auditEntitiesConfiguration = AuditConfiguration.AuditEntCfg;
			/*
			The query that should be executed in the versions table:
			SELECT e FROM ent_ver e, rev_entity r WHERE
			  e.revision_type != DEL (if selectDeletedEntities == false) AND
			  e.revision = r.revision AND
			  (all specified conditions, transformed, on the "e" entity)
			  ORDER BY e.revision ASC (unless another order is specified)
			 */
			SetIncludeDeletationClause();
			AddCriterions();
			AddOrders();
			QueryBuilder.AddFrom(auditEntitiesConfiguration.RevisionInfoEntityFullClassName(), QueryConstants.RevisionAlias, true);
			QueryBuilder.RootParameters.AddWhere(auditEntitiesConfiguration.RevisionNumberPath, true, "=", QueryConstants.RevisionAlias + ".id", false);

			var revisionTypePropertyName = auditEntitiesConfiguration.RevisionTypePropName;

			var result = BuildAndExecuteQuery<object[]>();
			return from resultRow in result
				   let versionsEntity = (IDictionary) resultRow[0]
				   let revisionData = (TRevisionEntity) resultRow[1]
				   let revision = GetRevisionNumberFromDynamicEntity(versionsEntity)
				   let entity = (TEntity) EntityInstantiator.CreateInstanceFromVersionsEntity(EntityName, versionsEntity, revision)
				   select new RevisionEntityInfo<TEntity, TRevisionEntity>(entity, revisionData, (RevisionType) versionsEntity[revisionTypePropertyName]);
		}
	}
}