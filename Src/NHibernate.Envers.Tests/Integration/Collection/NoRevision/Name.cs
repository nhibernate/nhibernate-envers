using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Collection.NoRevision
{
	[Audited]
	public class Name
	{
		public virtual int Id { get; set; }
		public virtual string TheName { get; set; }
		public virtual Person Person { get; set; }
	}
}