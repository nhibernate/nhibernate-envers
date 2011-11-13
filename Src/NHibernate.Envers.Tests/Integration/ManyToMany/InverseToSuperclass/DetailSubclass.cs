using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.ManyToMany.InverseToSuperclass
{
	[Audited]
	public class DetailSubclass : DetailSuperclass
	{
		public virtual string Str2 { get; set; }
	}
}