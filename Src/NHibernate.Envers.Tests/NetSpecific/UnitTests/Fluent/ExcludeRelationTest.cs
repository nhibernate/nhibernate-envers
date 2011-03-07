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
	public class ExcludeRelationTest
	{
		private IDictionary<System.Type, IEntityMeta> metas;

		[SetUp]
		public void Setup()
		{
			var cfg = new FluentConfiguration();
			cfg.Audit<NotAuditedOwnerEntity>()
					.ExcludeRelationData(s => s.Relation);
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
			var entMeta = metas[typeof(NotAuditedOwnerEntity)];
			entMeta.ClassMetas.OnlyContains<AuditedAttribute>();
		}

		[Test]
		public void PropertyShouldHaveOneAuditAttributeWithNoAuditedRelation()
		{
			var propInfo = typeof(NotAuditedOwnerEntity).GetProperty("Relation");
			var entMeta = metas[typeof(NotAuditedOwnerEntity)];

			entMeta.MemberMetas[propInfo]
				.Should().Have.Count.EqualTo(1);
			var auditAttr = (AuditedAttribute)entMeta.MemberMetas[propInfo].First();
			auditAttr.TargetAuditMode.Should().Be.EqualTo(RelationTargetAuditMode.NotAudited);
		}

		[Test]
		public void PropertyShouldHaveOneAuditAttributeWithNoAuditedRelation_using_field()
		{
			var cfg = new FluentConfiguration();
			cfg.Audit<NotAuditedOwnerEntity>()
					.ExcludeRelationData("RelationField");
			metas = cfg.CreateMetaData(null);

			var propInfo = typeof(NotAuditedOwnerEntity).GetField("RelationField");
			var entMeta = metas[typeof(NotAuditedOwnerEntity)];

			entMeta.MemberMetas[propInfo]
				.Should().Have.Count.EqualTo(1);
			var auditAttr = (AuditedAttribute)entMeta.MemberMetas[propInfo].First();
			auditAttr.TargetAuditMode.Should().Be.EqualTo(RelationTargetAuditMode.NotAudited);
		}
	}
}