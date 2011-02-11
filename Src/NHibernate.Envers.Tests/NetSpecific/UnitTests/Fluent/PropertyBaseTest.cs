using System.Collections.Generic;
using System.Reflection;
using NHibernate.Envers.Configuration.Fluent;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent.Model;
using NUnit.Framework;
using NHibernate.Envers.Tests.Tools;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent
{
	[TestFixture]
	public class PropertyBaseTest
	{
		private IDictionary<System.Type, IEntityMeta> metas;

		[SetUp]
		public void Setup()
		{
			var cfg = new FluentConfiguration();
			cfg.Audit<Dog>()
				.Exclude(dog => dog.Name)
				.Exclude("weight");
			metas = cfg.CreateMetaData(null);
		}

		[Test]
		public void NumberOfEntityMetas()
		{
			Assert.AreEqual(2, metas.Count);
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
		public void CanExcludeInBaseClass()
		{
			var weightRefl = typeof (Animal).GetField("weight", BindingFlags.Instance | BindingFlags.NonPublic);
			var nameRefl = typeof (Animal).GetProperty("Name");
			metas[typeof(Animal)].MemberMetas[weightRefl].OnlyContains<NotAuditedAttribute>();
			metas[typeof(Animal)].MemberMetas[nameRefl].OnlyContains<NotAuditedAttribute>();
		}
		
	}
}