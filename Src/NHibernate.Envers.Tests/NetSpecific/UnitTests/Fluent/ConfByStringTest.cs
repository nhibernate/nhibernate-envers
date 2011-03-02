using System.Collections.Generic;
using System.Reflection;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Fluent;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent.Model;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent
{
	[TestFixture]
	public class ConfByStringTest
	{
		private IDictionary<System.Type, IEntityMeta> metas;

		[SetUp]
		public void Setup()
		{
			var cfg = new FluentConfiguration();
			cfg.Audit<FieldEntity>()
				.Exclude("data1")
				.Exclude("data2");

			metas = cfg.CreateMetaData(null);
		}

		[Test]
		public void NumberOfEntityMetas()
		{
			Assert.AreEqual(1, metas.Count);
		}

		[Test]
		public void ClassShouldBeAudited()
		{
			var entMeta = metas[typeof(FieldEntity)];
			entMeta.ClassMetas.OnlyContains<AuditedAttribute>();
		}

		[Test]
		public void ExcludedPropertyShouldNotBeAudited()
		{
			var propInfo = typeof(FieldEntity).GetField("data1", BindingFlags.Instance | BindingFlags.NonPublic);
			var propInfo2 = typeof(FieldEntity).GetProperty("data2");
			var entMeta = metas[typeof(FieldEntity)];
			Assert.AreEqual(2, entMeta.MemberMetas.Count);
			entMeta.MemberMetas[propInfo].OnlyContains<NotAuditedAttribute>();
			entMeta.MemberMetas[propInfo2].OnlyContains<NotAuditedAttribute>();
		}

		[Test]
		public void IncorrectString()
		{
			var cfg = new FluentConfiguration();
			Assert.Throws<FluentException>(() =>
			                               		cfg.Audit<FieldEntity>()
			                               			.Exclude("data3")
			                               	);
		}
	}
}