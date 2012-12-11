using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Tests.NetSpecific.Integration.EntityInstantiation;
using NUnit.Framework;
using NHibernate.Envers.Configuration.Fluent;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Tests.Tools;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent
{
	[TestFixture]
	public class EntityInstantiationTest
	{
		private IDictionary<System.Type, IEntityMeta> metas;

		[SetUp]
		public void Setup()
		{
			var fluent = new FluentConfiguration();
			
			fluent.Audit<TestEntityWithContext>();
			fluent.Audit<FactoryCreatedTestEntity>()
				.UseFactory(new TestEntityFactory());

			metas = fluent.CreateMetaData(null);
		}

		[Test]
		public void StdEntityIsNotCreatedByFactory()
		{
			var entMeta = metas[typeof(TestEntityWithContext)];
			entMeta.ClassMetas.OnlyContains<AuditedAttribute>();
		}

		[Test]
		public void FactoryAttributeIsApplied()
		{
			var entMeta = metas[typeof(FactoryCreatedTestEntity)];
			Assert.IsNotNull(entMeta.ClassMetas.FirstOrDefault(x => x is AuditFactoryAttribute));
		}
	}
}
