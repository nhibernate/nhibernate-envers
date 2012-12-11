using System;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.EntityInstantiation
{
	public class TestEntityFactory : IEntityFactory<FactoryCreatedTestEntity>
	{
		public FactoryCreatedTestEntity Instantiate()
		{
			return new FactoryCreatedTestEntity(true);
		}

		public object Instantiate(System.Type type)
		{
			if (type == null) throw new ArgumentNullException("type");
			if(!type.Equals(typeof(FactoryCreatedTestEntity))) throw new InvalidOperationException("Invalid type supplied to entity factory");

			return Instantiate();
		}
	}
}
