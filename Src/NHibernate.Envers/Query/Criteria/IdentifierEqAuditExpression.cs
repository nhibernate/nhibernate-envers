using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Criteria
{
	/// <summary>
	/// A criterion that expresses that the id of an entity is equal or not equal to some specified value.
	/// </summary>
	public class IdentifierEqAuditExpression : IAuditCriterion
	{
		private readonly object id;
		private readonly bool equals;

		public IdentifierEqAuditExpression(object id, bool equals)
		{
			this.id = id;
			this.equals = equals;
		}

		public void AddToQuery(AuditConfiguration verCfg, IAuditReaderImplementor versionsReader, string entityName, QueryBuilder qb, Parameters parameters)
		{
			verCfg.EntCfg[entityName].IdMapper
					  .AddIdEqualsToQuery(parameters, id, verCfg.AuditEntCfg.OriginalIdPropName, equals);
		}
	}
}
