using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.CustomType
{
	public class PrimitiveCustomTypeEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual PrimitiveImmutableType PrimitiveType { get; set; }
	}
}
