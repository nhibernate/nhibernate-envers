namespace NHibernate.Envers.Tests.NetSpecific.Integration.OneToOne
{
	[Audited]    
	public class OneToOneOwningEntity
	{
		public virtual int Id { get; set; }
		public virtual OneToOneOwnedEntity Owned { get; set; }
	}
}