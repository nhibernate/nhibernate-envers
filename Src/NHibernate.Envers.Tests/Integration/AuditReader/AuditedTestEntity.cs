using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.AuditReader
{
	public class AuditedTestEntity
	{
		public virtual int Id { get; set; }
		[Audited]
		public virtual string Data { get; set; }
	}
}