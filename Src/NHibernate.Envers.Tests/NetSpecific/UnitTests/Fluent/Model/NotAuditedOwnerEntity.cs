namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent.Model
{
	public class NotAuditedOwnerEntity
	{
		public virtual NotAuditedEntity Relation { get; set; }
	}
}