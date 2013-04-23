using System;
using NHibernate.Envers.Query.Property;

namespace NHibernate.Envers.Query.Criteria
{
	/// <summary>
	/// Create restrictions and projections for the id of an audited entity.
	/// </summary>
	public class AuditId : AuditProperty
	{
		public const string IdentifierPlaceholder = "$$id$$";
		private static readonly IPropertyNameGetter identifierPropertyGetter = new EntityPropertyName(IdentifierPlaceholder);

		public AuditId() : base(identifierPropertyGetter)
		{
		}

		public override IAuditCriterion Eq(object id) 
		{
			return new IdentifierEqAuditExpression(id, true);
		}

		public override IAuditCriterion Ne(object id) 
		{
			return new IdentifierEqAuditExpression(id, false);
		}

		public override IAuditCriterion HasChanged()
		{
			throw new NotSupportedException();
		}

		public override IAuditCriterion HasNotChanged()
		{
			throw new NotSupportedException();
		}
	}
}
