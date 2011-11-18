using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Interfaces.Inheritance.AllAudited
{
	[Audited]
	public class AuditedImplementor : ISimple
	{
		public virtual long Id { get; set; }
		public virtual string Data { get; set; }
		public virtual string AuditedImplementorData { get; set; }
	}
}