namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent.Model
{
	public class NotAuditedOwnerEntity
	{
		public NotAuditedEntity RelationField;
		public virtual NotAuditedEntity Relation { get; set; }
	}
}