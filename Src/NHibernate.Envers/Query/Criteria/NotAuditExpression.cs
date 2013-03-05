using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Criteria
{
	public class NotAuditExpression : IAuditCriterion
	{
		private readonly IAuditCriterion criterion;

		public NotAuditExpression(IAuditCriterion criterion)
		{
			this.criterion = criterion;
		}

		public void AddToQuery(AuditConfiguration verCfg, IAuditReaderImplementor versionsReader, string entityName, QueryBuilder qb, Parameters parameters)
		{
			criterion.AddToQuery(verCfg, versionsReader, entityName, qb, parameters.AddNegatedParameters());
		}
	}
}
