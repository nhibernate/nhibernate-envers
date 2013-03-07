using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Criteria
{
	public class RevisionTypeAuditExpression : IAuditCriterion
	{
		private readonly object _value;
		private readonly string _op;

		public RevisionTypeAuditExpression(object value, string op)
		{
			_value = value;
			_op = op;
		}

		public void AddToQuery(AuditConfiguration auditCfg, IAuditReaderImplementor versionsReader, string entityName, QueryBuilder qb, Parameters parameters)
		{
			parameters.AddWhereWithParam(auditCfg.AuditEntCfg.RevisionTypePropName, _op, _value);
		}
	}
}