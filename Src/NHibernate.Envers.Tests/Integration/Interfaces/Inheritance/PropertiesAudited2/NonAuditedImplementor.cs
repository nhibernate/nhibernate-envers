namespace NHibernate.Envers.Tests.Integration.Interfaces.Inheritance.PropertiesAudited2
{
	public class NonAuditedImplementor : ISimple
	{
		public virtual long Id { get; set; }
		public virtual string Data { get; set; }
		public virtual int Number { get; set; }
		public virtual string NonAuditedImplementorData { get; set; }
	}
}