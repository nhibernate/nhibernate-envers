using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.CustomType
{
	public class CLOBCustomTypeEntity
	{
		public virtual int Id { get; set; }
		[Audited]
		public virtual string Str { get; set; }
	}
}