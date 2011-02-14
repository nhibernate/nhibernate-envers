namespace NHibernate.Envers.Tests.NetSpecific.Integration.OneToOne
{
	[Audited]
	public class OneToOneOwnedEntity
	{
		public virtual int Id { get; set; }
		public virtual OneToOneOwningEntity Owning { get; set; }
	}
}