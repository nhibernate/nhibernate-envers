using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Query.Impl
{
	public partial class RevisionsQuery<TEntity> : AbstractRevisionsQuery<TEntity> where TEntity : class
	{
		public RevisionsQuery(AuditConfiguration auditConfiguration,
		                      IAuditReaderImplementor versionsReader,
		                      bool includesDeletations) : base(auditConfiguration, versionsReader, includesDeletations, typeof (TEntity).FullName) {}

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
			var result = BuildAndExecuteQuery<IDictionary>();
			return from versionsEntity in result
			       let revision = GetRevisionNumberFromDynamicEntity(versionsEntity)
			       select (TEntity) EntityInstantiator.CreateInstanceFromVersionsEntity(EntityName, versionsEntity, revision);
		}
	}
}