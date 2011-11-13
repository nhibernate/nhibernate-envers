using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.OneToMany.InverseToSuperclass
{
	[Audited]
	public class DetailSuperclass
	{
		public virtual long Id { get; set; }
		public virtual Master Master { get; set; }
	}
}