using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Fluent;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent.Model;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent
{
	[TestFixture]
	public class InheritedRevisionInfoTest
	{
		private IDictionary<System.Type, IEntityMeta> metas;

		[SetUp]
		public void Setup()
		{
			var cfg = new FluentConfiguration();
			cfg.SetRevisionEntity<InheritedRevisionEntity>(e => e.Id, e => e.RevisionDate);
			metas = cfg.CreateMetaData(FakeNHibernateConfiguration.For<InheritedRevisionEntity>());
		}

		[Test]
		public void NumberOfEntityMetas()
		{
			Assert.AreEqual(1, metas.Count);
		}

		[Test]
		public void RevisionEntityAttributeShouldNotHaveDefaultListenerSet()
		{
			var entMeta = metas[typeof(InheritedRevisionEntity)];
			var revEntAttr = (RevisionEntityAttribute)entMeta.ClassMetas.First();
			revEntAttr.Listener
			  .Should().Be.Null();
		}

		[Test]
		public void RevisionEntityShouldBeSet()
		{
			var entMeta = metas[typeof(InheritedRevisionEntity)];
			entMeta.ClassMetas.OnlyContains<RevisionEntityAttribute>();
		}

		[Test]
		public void RevisionPropertiesShouldBeenSet()
		{
			var revType = typeof(InheritedRevisionEntity);
			var entMeta = metas[revType];
			var propMeta = entMeta.MemberMetas;
			Assert.AreEqual(2, propMeta.Count);
			propMeta[revType.GetProperty("Id")].OnlyContains<RevisionNumberAttribute>();
			propMeta[revType.GetProperty("RevisionDate")].OnlyContains<RevisionTimestampAttribute>();
		}
	}
}