using System.Collections.Generic;
using NHibernate.Envers.Configuration.Fluent;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;
using NHibernate.Envers.Tests.Tools;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent
{
	[TestFixture]
	public class ExcludeTest
	{
		private IDictionary<System.Type, IEntityMeta> metas;

		[SetUp]
		public void Setup()
		{
			var cfg = new FluentConfiguration();
			cfg.Audit<StrTestEntity>()
					.Exclude(s => s.Str);
			metas = cfg.CreateMetaData(null);
		}

		[Test]
		public void NumberOfEntities()
		{
			Assert.AreEqual(1, metas.Count);
		}

		[Test]
		public void ClassShouldBeAudited()
		{
			var entMeta = metas[typeof(StrTestEntity)];
			entMeta.ClassMetas.OnlyContains<AuditedAttribute>();
		}

		[Test]
		public void ExcludedPropertyShouldNotBeAudited()
		{
			var propInfo = typeof (StrTestEntity).GetProperty("Str");
			var entMeta = metas[typeof(StrTestEntity)];
			Assert.AreEqual(1, entMeta.MemberMetas.Count);
			entMeta.MemberMetas[propInfo].OnlyContains<NotAuditedAttribute>();
		}
	}
}