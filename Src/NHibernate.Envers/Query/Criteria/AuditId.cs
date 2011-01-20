using NHibernate.Envers.Query.Projection;
using NHibernate.Envers.Query.Property;

namespace NHibernate.Envers.Query.Criteria
{
	/// <summary>
	/// Create restrictions and projections for the id of an audited entity.
	/// </summary>
	public class AuditId 
	{
		public IAuditCriterion Eq(object id) 
		{
			return new IdentifierEqAuditExpression(id, true);
		}

		public IAuditCriterion Ne(object id) 
		{
			return new IdentifierEqAuditExpression(id, false);
		}

		/**
		 * Projection counting the values
		 * TODO: idPropertyName isn't needed, should be read from the configuration
		 * @param idPropertyName Name of the identifier property
		 */
		public IAuditProjection Count(string idPropertyName) 
		{
			return new PropertyAuditProjection(new OriginalIdPropertyName(idPropertyName), "count", false);
		}
	}
}
