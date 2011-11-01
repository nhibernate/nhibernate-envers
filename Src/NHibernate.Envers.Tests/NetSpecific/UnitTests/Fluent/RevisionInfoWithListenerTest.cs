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
	public class RevisionInfoWithListenerTest
	{
		private IDictionary<System.Type, IEntityMeta> metas;
		private IRevisionListener revisionListener;

		[SetUp]
		public void Setup()
		{
			revisionListener = new RevListener();
			var cfg = new FluentConfiguration();
			cfg.SetRevisionEntity<RevisionEntity>(e => e.Number, e => e.Timestamp, revisionListener);
			metas = cfg.CreateMetaData(FakeNHibernateConfiguration.For<RevisionEntity>());
		}

		[Test]
		public void NumberOfEntityMetas()
		{
			Assert.AreEqual(1, metas.Count);
		}

		[Test]
		public void RevisionEntityShouldBeSet()
		{
			var entMeta = metas[typeof(RevisionEntity)];
			entMeta.ClassMetas.OnlyContains<RevisionEntityAttribute>();
		}

		[Test]
		public void RevisionEntityAttributeShouldHaveListenerSet()
		{
			var entMeta = metas[typeof(RevisionEntity)];
			var revEntAttr = (RevisionEntityAttribute)entMeta.ClassMetas.First();
			revEntAttr.Listener
					.Should().Be.SameInstanceAs(revisionListener);
		}

		[Test]
		public void RevisionPropertiesShouldBeenSet()
		{
			var revType = typeof(RevisionEntity);
			var entMeta = metas[revType];
			var propMeta = entMeta.MemberMetas;
			Assert.AreEqual(2, propMeta.Count);
			propMeta[revType.GetProperty("Number")].OnlyContains<RevisionNumberAttribute>();
			propMeta[revType.GetProperty("Timestamp")].OnlyContains<RevisionTimestampAttribute>();
		}
	}
}