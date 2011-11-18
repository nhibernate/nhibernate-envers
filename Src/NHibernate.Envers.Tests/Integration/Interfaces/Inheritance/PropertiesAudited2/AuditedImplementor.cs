using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Interfaces.Inheritance.PropertiesAudited2
{
	[Audited]
	public class AuditedImplementor : ISimple
	{
		public virtual long Id { get; set; }
		public virtual string Data { get; set; }
		public virtual int Number { get; set; }
		public virtual string AuditedImplementorData { get; set; }
	}
}