﻿using System.Collections.Generic;
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
	public class RevisionInfoWithModifiedEntityNamesAndListenerTest
	{
		private IDictionary<System.Type, IEntityMeta> metas;
		private IRevisionListener revisionListener;

		[SetUp]
		public void Setup()
		{
			revisionListener = new RevListener();
			var cfg = new FluentConfiguration();
			cfg.SetRevisionEntity<RevisionEntityWithEntityNames>(e => e.Number, e => e.Timestamp, e => e.EntityNames, revisionListener);
			metas = cfg.CreateMetaData(FakeNHibernateConfiguration.For<RevisionEntityWithEntityNames>());
		}

		[Test]
		public void NumberOfEntityMetas()
		{
			Assert.AreEqual(1, metas.Count);
		}

		[Test]
		public void RevisionEntityShouldBeSet()
		{
			var entMeta = metas[typeof(RevisionEntityWithEntityNames)];
			entMeta.ClassMetas.OnlyContains<RevisionEntityAttribute>();
		}

		[Test]
		public void RevisionEntityAttributeShouldHaveListenerSet()
		{
			var entMeta = metas[typeof(RevisionEntityWithEntityNames)];
			var revEntAttr = (RevisionEntityAttribute)entMeta.ClassMetas.First();
			revEntAttr.Listener
							.Should().Be.SameInstanceAs(revisionListener);
		}

		[Test]
		public void RevisionPropertiesShouldBeenSet()
		{
			var revType = typeof(RevisionEntityWithEntityNames);
			var entMeta = metas[revType];
			var propMeta = entMeta.MemberMetas;
			Assert.AreEqual(3, propMeta.Count);
			propMeta[revType.GetProperty("Number")].OnlyContains<RevisionNumberAttribute>();
			propMeta[revType.GetProperty("Timestamp")].OnlyContains<RevisionTimestampAttribute>();
			propMeta[revType.GetProperty("EntityNames")].OnlyContains<ModifiedEntityNamesAttribute>();
		}
	}
}