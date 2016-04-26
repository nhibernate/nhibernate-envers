using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Criteria
{
	public interface IAuditCriterion
	{
		void AddToQuery(AuditConfiguration auditCfg, IAuditReaderImplementor versionsReader, string entityName, string alias, QueryBuilder qb, Parameters parameters);
	}
}
