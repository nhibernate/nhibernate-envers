using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.EntityNames.SingleAssociatedNotAudited
{
	public class Car
	{
		public virtual long Id { get; set; }
		[Audited]
		public virtual int Number { get; set; }
		[Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
		public virtual Person Owner { get; set; }
	}
}