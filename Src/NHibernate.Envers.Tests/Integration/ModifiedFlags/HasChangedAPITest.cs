using System.Collections.Generic;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Integration.AuditReader;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class HasChangedAPITest : AbstractModifiedFlagsEntityTest
	{
		public HasChangedAPITest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ent1 = new AuditedTestEntity {Id = 1, Data = "str1"};
			var ent2 = new NotAuditedTestEntity {Id = 1, Data = "str1"};
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ent1);
				Session.Save(ent2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ent1.Data = "str2";
				ent2.Data = "str2";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyHasChangedCriteria()
		{
			var list = AuditReader().CreateQuery().ForRevisionsOfEntity(typeof (AuditedTestEntity), true, true)
				.Add(AuditEntity.Property("Data").HasChanged()).GetResultList<AuditedTestEntity>();
			list.Should().Have.Count.EqualTo(2);
			list[0].Data.Should().Be.EqualTo("str1");
			list[1].Data.Should().Be.EqualTo("str2");
		}

		[Test]
		public void VerifyHasNotChangedCriteria()
		{
			var list = AuditReader().CreateQuery().ForRevisionsOfEntity(typeof (AuditedTestEntity), true, true)
				.Add(AuditEntity.Property("Data").HasNotChanged()).GetResultList<AuditedTestEntity>();
			list.Should().Be.Empty();
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Integration.AuditReader.Mapping.hbm.xml" }; }
		}
	}
}