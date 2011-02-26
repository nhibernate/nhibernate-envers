using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;
using NHibernate.Proxy;

namespace NHibernate.Envers.Query.Impl
{
	public class RevisionsQuery<TEntity> : AbstractRevisionsQuery<TEntity> where TEntity : class
	{
		public RevisionsQuery(AuditConfiguration auditConfiguration,
		                      IAuditReaderImplementor versionsReader,
		                      bool includesDeletations) : base(auditConfiguration, versionsReader, includesDeletations) {}

		public override IEnumerable<TEntity> Results()
		{
			/*
			The query that should be executed in the versions table:
			SELECT e FROM ent_ver e, rev_entity r WHERE
			  e.revision_type != DEL (if includesDeletations == false) AND
			  e.revision = r.revision AND
			  (all specified conditions, transformed, on the "e" entity)
			  ORDER BY e.revision ASC (unless another order is specified)
			 */
			SetIncludeDeletationClause();

			AddCriterions();

			AddOrders();

			// the result of BuildAndExecuteQuery is always the name-value pair of EntityMode.Map
			return from versionsEntity in BuildAndExecuteQuery<IDictionary>()
			       let revision = GetRevisionNumberFromDynamicEntity(versionsEntity)
			       select (TEntity) EntityInstantiator.CreateInstanceFromVersionsEntity(EntityName, versionsEntity, revision);
		}

		private long GetRevisionNumberFromDynamicEntity(IDictionary versionsEntity)
		{
			var auditEntitiesConfiguration = AuditConfiguration.AuditEntCfg;
			string originalId = auditEntitiesConfiguration.OriginalIdPropName;
			string revisionPropertyName = auditEntitiesConfiguration.RevisionFieldName;
			object revisionInfoObject = ((IDictionary) versionsEntity[originalId])[revisionPropertyName];
			var proxy = revisionInfoObject as INHibernateProxy; // TODO: usage of proxyfactory IsProxy

			return proxy != null ? Convert.ToInt64(proxy.HibernateLazyInitializer.Identifier) : AuditConfiguration.RevisionInfoNumberReader.RevisionNumber(revisionInfoObject);
		}
	}
}