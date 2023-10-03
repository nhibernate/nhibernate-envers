using System.Collections.Generic;
using NHibernate.Envers.Exceptions;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.SuperClass.AuditedAtSuperclassLevel.AuditAllSubclass
{
	public partial class MappedSubclassingAllAuditedTest : TestBase
	{
		private int id1_1;
		private int id2_1;

		public MappedSubclassingAllAuditedTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var nas = new NotAuditedSubclassEntity { Str = "nae", OtherStr = "super str", NotAuditedStr = "not audited str" };
			var ae = new AuditedAllSubclassEntity { Str = "ae", OtherStr = "super str", SubAuditedStr = "audited str" };

			//revision 1
			using (var tx = Session.BeginTransaction())
			{
				id1_1 = (int)Session.Save(ae);
				id2_1 = (int)Session.Save(nas);
				tx.Commit();
			}

			//revision 2
			using (var tx = Session.BeginTransaction())
			{
				ae.Str = "ae new";
				ae.SubAuditedStr = "audited str new";
				nas.Str = "nae new";
				nas.NotAuditedStr = "not aud str new";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionsCountsForAudited()
		{
			AuditReader().GetRevisions(typeof(AuditedAllSubclassEntity), id1_1)
				.Should().Have.SameSequenceAs(1, 2);
		}

		[Test]
		public void VerifyRevisionsCountsForNotAudited()
		{
			Assert.Throws<NotAuditedException>(() =>
				AuditReader().GetRevisions(typeof(NotAuditedSubclassEntity), id2_1));
		}

		[Test]
		public void VerifyHistoryOfAudited()
		{
			var ver1 = new AuditedAllSubclassEntity { Id = id1_1, Str = "ae", OtherStr = "super str", SubAuditedStr = "audited str" };
			var ver2 = new AuditedAllSubclassEntity { Id = id1_1, Str = "ae new", OtherStr = "super str", SubAuditedStr = "audited str new" };

			var rev1 = AuditReader().Find<AuditedAllSubclassEntity>(id1_1, 1);
			var rev2 = AuditReader().Find<AuditedAllSubclassEntity>(id1_1, 2);

			rev1.OtherStr.Should().Be.EqualTo(ver1.OtherStr);
			rev2.OtherStr.Should().Be.EqualTo(ver2.OtherStr);

			rev1.Should().Be.EqualTo(ver1);
			rev2.Should().Be.EqualTo(ver2);
		}

		[Test]
		public void VerifyHistoryOfNotAudited()
		{
			Assert.Throws<NotAuditedException>(() =>
				AuditReader().Find(typeof(NotAuditedSubclassEntity), id2_1, 1));
		}


		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[]
				       	{
				       		"Integration.SuperClass.AuditedAtSuperclassLevel.AuditAllSubclass.Mapping.hbm.xml",
				       		"Integration.SuperClass.AuditedAtSuperclassLevel.Mapping.hbm.xml"
				       	};
			}
		}
	}
}