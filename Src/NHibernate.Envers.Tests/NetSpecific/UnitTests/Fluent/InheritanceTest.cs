using System;
using System.Collections.Generic;
using NHibernate.Envers.Configuration.Fluent;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent.Model;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent
{
	[TestFixture]
	public class InheritanceTest
	{
		private IDictionary<System.Type, IEntityMeta> metas;

		[SetUp]
		public void Setup()
		{
			var cfg = new FluentConfiguration();
			cfg.Audit<Dog>();
			cfg.Audit<Cat>();
			metas = cfg.CreateMetaData(null);
		}

		[Test]
		public void NumberOfEntityMetas()
		{
			Assert.AreEqual(3, metas.Count);
		}

		[Test]
		public void AnimalShouldBeAudited()
		{
			var entMeta = metas[typeof(Animal)];
			entMeta.ClassMetas.OnlyContains<AuditedAttribute>();
		}

		[Test]
		public void DogShouldBeAudited()
		{
			var entMeta = metas[typeof(Dog)];
			entMeta.ClassMetas.OnlyContains<AuditedAttribute>();
		}

		[Test]
		public void CatShouldBeAudited()
		{
			var entMeta = metas[typeof(Cat)];
			entMeta.ClassMetas.OnlyContains<AuditedAttribute>();
		}
	}
}