using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Fluent;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent.Model;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;

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
			cfg.Audit<SomePropsEntity>()
					.Exclude(s => s.Number)
					.Exclude(s => s.String)
					.Exclude(s => s.FieldNumber);
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
			var entMeta = metas[typeof(SomePropsEntity)];
			entMeta.ClassMetas.OnlyContains<AuditedAttribute>();
		}

		[Test]
		public void ExcludedPropertyShouldNotBeAudited()
		{
			var propInfo = typeof(SomePropsEntity).GetProperty("Number");
			var propInfo2 = typeof(SomePropsEntity).GetProperty("String");
			var fieldInfo = typeof (SomePropsEntity).GetField("FieldNumber");
			var entMeta = metas[typeof(SomePropsEntity)];
			Assert.AreEqual(3, entMeta.MemberMetas.Count);
			entMeta.MemberMetas[propInfo].OnlyContains<NotAuditedAttribute>();
			entMeta.MemberMetas[propInfo2].OnlyContains<NotAuditedAttribute>();
			entMeta.MemberMetas[fieldInfo].OnlyContains<NotAuditedAttribute>();
		}
	}
}