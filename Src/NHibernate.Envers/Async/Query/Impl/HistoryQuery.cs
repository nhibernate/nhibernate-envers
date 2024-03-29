﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Query.Impl
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class HistoryQuery<TEntity, TRevisionEntity> : AbstractRevisionsQuery<IRevisionEntityInfo<TEntity, TRevisionEntity>>
		where TEntity : class
		where TRevisionEntity : class
	{

		public override async Task<IEnumerable<IRevisionEntityInfo<TEntity, TRevisionEntity>>> ResultsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
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

			var result = await (BuildAndExecuteQueryAsync<object[]>(cancellationToken)).ConfigureAwait(false);
			return from resultRow in result
				   let versionsEntity = (IDictionary) resultRow[0]
				   let revisionData = (TRevisionEntity) resultRow[1]
				   let revision = GetRevisionNumberFromDynamicEntity(versionsEntity)
				   let entity = (TEntity) EntityInstantiator.CreateInstanceFromVersionsEntity(EntityName, versionsEntity, revision)
				   select new RevisionEntityInfo<TEntity, TRevisionEntity>(entity, revisionData, (RevisionType) versionsEntity[revisionTypePropertyName]);
		}
	}
}