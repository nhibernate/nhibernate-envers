using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Ctor
{
	[Audited]
	public class NonPublicCtorEntity
	{
		protected NonPublicCtorEntity() { }

		public NonPublicCtorEntity(NonPublicCtorComponent component, StructComponentWithDefinedCtor structComponent)
		{
			Component = component;
			StructComponent = structComponent;
		}

		public virtual int Id { get; set; }
		public virtual NonPublicCtorComponent Component { get; set; }
		public virtual StructComponentWithDefinedCtor StructComponent { get; set; }
	}
}