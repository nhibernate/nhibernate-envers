namespace NHibernate.Envers.Tests.Integration.Interfaces.Inheritance.AllAudited
{
	public class NonAuditedImplementor : ISimple
	{
		public virtual long Id { get; set; }
		public virtual string Data { get; set; }
		public virtual string NonAuditedImplementorData { get; set; }
	}
}