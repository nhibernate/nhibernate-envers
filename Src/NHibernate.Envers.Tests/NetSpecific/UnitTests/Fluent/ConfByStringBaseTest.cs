using System.Collections.Generic;
using System.Reflection;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent.Model;
using NHibernate.Envers.Tests.Tools;
using NHibernate.Util;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent
{
	public abstract class ConfByStringBaseTest
	{
		protected IDictionary<System.Type, IEntityMeta> metas;

		[Test]
		public void NumberOfEntityMetas()
		{
			Assert.AreEqual(2, metas.Count);
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
		public void ExcludedDataPropertyShouldNotBeAudited()
		{
			var propInfo = typeof (NotAuditedOwnerEntity).GetField("RelationField");
			var entMeta = metas[typeof (NotAuditedOwnerEntity)];
			entMeta.MemberMetas.Should().Have.Count.EqualTo(1);
			entMeta.MemberMetas[propInfo].Should().Have.Count.EqualTo(1);
			var attr = (AuditedAttribute)entMeta.MemberMetas[propInfo].First();
			attr.TargetAuditMode.Should().Be.EqualTo(RelationTargetAuditMode.NotAudited);
		}
	}

}
