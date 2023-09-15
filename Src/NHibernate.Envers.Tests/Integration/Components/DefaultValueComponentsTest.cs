using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Components;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Components
{
	public partial class DefaultValueComponentsTest : TestBase
	{
		private int id0;
		private int id1;
		private int id2;
		private int id3;
		private int id4;
		private int id5;
		private int id6;

		public DefaultValueComponentsTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var cte0 = new DefaultValueComponentTestEntity();
			var cte1 = new DefaultValueComponentTestEntity { Comp1 = new DefaultValueComponent1 { Str1 = "c1-str1", Comp2 = null} };
			var cte2 = new DefaultValueComponentTestEntity { Comp1 = new DefaultValueComponent1 { Str1 = "c1-str1", Comp2 = new DefaultValueComponent2 { Str1 = "c2-str1", Str2 = "c2-str2" } } };
			var cte3 = new DefaultValueComponentTestEntity { Comp1 = new DefaultValueComponent1 { Str1 = null, Comp2 = new DefaultValueComponent2 { Str1 = "c2-str1", Str2 = "c2-str2" } } };
			var cte4 = new DefaultValueComponentTestEntity { Comp1 = new DefaultValueComponent1 { Str1 = null, Comp2 = new DefaultValueComponent2 { Str1 = null, Str2 = "c2-str2" } } };
			var cte5 = new DefaultValueComponentTestEntity { Comp1 = new DefaultValueComponent1 { Str1 = null, Comp2 = new DefaultValueComponent2 { Str1 = "c2-str1", Str2 = null } } };
			var cte6 = new DefaultValueComponentTestEntity { Comp1 = new DefaultValueComponent1 { Str1 = null, Comp2 = new DefaultValueComponent2 { Str1 = null, Str2 = null } } };

	
			//revision 1
			using (var tx = Session.BeginTransaction())
			{
				id0 = (int) Session.Save(cte0);
				id1 = (int) Session.Save(cte1);
				id2 = (int) Session.Save(cte2);
				id3 = (int) Session.Save(cte3);
				id4 = (int) Session.Save(cte4);
				id5 = (int) Session.Save(cte5);
				id6 = (int) Session.Save(cte6);
				tx.Commit();
			}

			//revision 2
			using (var tx = Session.BeginTransaction())
			{
				cte0.Comp1 = new DefaultValueComponent1 {Str1 = "upd-c1-str1", Comp2 = null};
				cte1.Comp1 = new DefaultValueComponent1 { Str1 = null, Comp2 = new DefaultValueComponent2 { Str1 = "upd-c2-str1", Str2 = "upd-c2-str2" } };
				cte2.Comp1.Comp2.Str1 = "upd-c2-str1";
				cte3.Comp1.Comp2.Str1 = "upd-c2-str1";
				cte4.Comp1.Comp2.Str1 = "upd-c2-str1";
				cte5.Comp1.Comp2.Str1 = "upd-c2-str1";
				cte6.Comp1.Comp2.Str1 = "upd-c2-str1";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			AuditReader().GetRevisions(typeof (DefaultValueComponentTestEntity), id0).Should().Have.SameSequenceAs(1, 2);
			AuditReader().GetRevisions(typeof (DefaultValueComponentTestEntity), id1).Should().Have.SameSequenceAs(1, 2);
			AuditReader().GetRevisions(typeof (DefaultValueComponentTestEntity), id2).Should().Have.SameSequenceAs(1, 2);
			AuditReader().GetRevisions(typeof (DefaultValueComponentTestEntity), id3).Should().Have.SameSequenceAs(1, 2);
			AuditReader().GetRevisions(typeof (DefaultValueComponentTestEntity), id4).Should().Have.SameSequenceAs(1, 2);
			AuditReader().GetRevisions(typeof (DefaultValueComponentTestEntity), id5).Should().Have.SameSequenceAs(1, 2);
			AuditReader().GetRevisions(typeof (DefaultValueComponentTestEntity), id6).Should().Have.SameSequenceAs(1, 2);
		}

		[Test]
		public void VerifyHistoryOfId0()
		{
			var ent1 = AuditReader().Find<DefaultValueComponentTestEntity>(id0, 1);
			var ent2 = AuditReader().Find<DefaultValueComponentTestEntity>(id0, 2);
			var expectedVer1 = new DefaultValueComponentTestEntity {Id = id0, Comp1 = new DefaultValueComponent1{Str1 = null, Comp2 = null}};
			var expectedVer2 = new DefaultValueComponentTestEntity { Id = id0, Comp1 = new DefaultValueComponent1 { Str1 = "upd-c1-str1", Comp2 = null } };
			ent1.Should().Be.EqualTo(expectedVer1);
			ent2.Should().Be.EqualTo(expectedVer2);
		}

		[Test]
		public void VerifyHistoryOfId1()
		{
			var ent1 = AuditReader().Find<DefaultValueComponentTestEntity>(id1, 1);
			var ent2 = AuditReader().Find<DefaultValueComponentTestEntity>(id1, 2);
			var expectedVer1 = new DefaultValueComponentTestEntity { Id = id1, Comp1 = new DefaultValueComponent1 { Str1 = "c1-str1", Comp2 = null } };
			var expectedVer2 = new DefaultValueComponentTestEntity { Id = id1, Comp1 = new DefaultValueComponent1 { Str1 = null, Comp2 = new DefaultValueComponent2 { Str1 = "upd-c2-str1", Str2 = "upd-c2-str2" } } };
			ent1.Should().Be.EqualTo(expectedVer1);
			ent2.Should().Be.EqualTo(expectedVer2);
		}

		[Test]
		public void VerifyHistoryOfId2()
		{
			var ent1 = AuditReader().Find<DefaultValueComponentTestEntity>(id2, 1);
			var ent2 = AuditReader().Find<DefaultValueComponentTestEntity>(id2, 2);
			var expectedVer1 = new DefaultValueComponentTestEntity { Id = id2, Comp1 = new DefaultValueComponent1 { Str1 = "c1-str1", Comp2 = new DefaultValueComponent2 { Str1 = "c2-str1", Str2 = "c2-str2" } } };
			var expectedVer2 = new DefaultValueComponentTestEntity { Id = id2, Comp1 = new DefaultValueComponent1 { Str1 = "c1-str1", Comp2 = new DefaultValueComponent2 { Str1 = "upd-c2-str1", Str2 = "c2-str2" } } };
			ent1.Should().Be.EqualTo(expectedVer1);
			ent2.Should().Be.EqualTo(expectedVer2);
		}

		[Test]
		public void VerifyHistoryOfId3()
		{
			var ent1 = AuditReader().Find<DefaultValueComponentTestEntity>(id3, 1);
			var ent2 = AuditReader().Find<DefaultValueComponentTestEntity>(id3, 2);
			var expectedVer1 = new DefaultValueComponentTestEntity { Id = id3, Comp1 = new DefaultValueComponent1 { Str1 = null, Comp2 = new DefaultValueComponent2 { Str1 = "c2-str1", Str2 = "c2-str2" } } };
			var expectedVer2 = new DefaultValueComponentTestEntity { Id = id3, Comp1 = new DefaultValueComponent1 { Str1 = null, Comp2 = new DefaultValueComponent2 { Str1 = "upd-c2-str1", Str2 = "c2-str2" } } };
			ent1.Should().Be.EqualTo(expectedVer1);
			ent2.Should().Be.EqualTo(expectedVer2);
		}

		[Test]
		public void VerifyHistoryOfId4()
		{
			var ent1 = AuditReader().Find<DefaultValueComponentTestEntity>(id4, 1);
			var ent2 = AuditReader().Find<DefaultValueComponentTestEntity>(id4, 2);
			var expectedVer1 = new DefaultValueComponentTestEntity { Id = id4, Comp1 = new DefaultValueComponent1 { Str1 = null, Comp2 = new DefaultValueComponent2 { Str1 = null, Str2 = "c2-str2" } } };
			var expectedVer2 = new DefaultValueComponentTestEntity { Id = id4, Comp1 = new DefaultValueComponent1 { Str1 = null, Comp2 = new DefaultValueComponent2 { Str1 = "upd-c2-str1", Str2 = "c2-str2" } } };
			ent1.Should().Be.EqualTo(expectedVer1);
			ent2.Should().Be.EqualTo(expectedVer2);
		}

		[Test]
		public void VerifyHistoryOfId5()
		{
			var ent1 = AuditReader().Find<DefaultValueComponentTestEntity>(id5, 1);
			var ent2 = AuditReader().Find<DefaultValueComponentTestEntity>(id5, 2);
			var expectedVer1 = new DefaultValueComponentTestEntity { Id = id5, Comp1 = new DefaultValueComponent1 { Str1 = null, Comp2 = new DefaultValueComponent2 { Str1 = "c2-str1", Str2 = null } } };
			var expectedVer2 = new DefaultValueComponentTestEntity { Id = id5, Comp1 = new DefaultValueComponent1 { Str1 = null, Comp2 = new DefaultValueComponent2 { Str1 = "upd-c2-str1", Str2 = null } } };
			ent1.Should().Be.EqualTo(expectedVer1);
			ent2.Should().Be.EqualTo(expectedVer2);
		}

		[Test]
		public void VerifyHistoryOfId6()
		{
			var ent1 = AuditReader().Find<DefaultValueComponentTestEntity>(id6, 1);
			var ent2 = AuditReader().Find<DefaultValueComponentTestEntity>(id6, 2);
			var expectedVer1 = new DefaultValueComponentTestEntity { Id = id6, Comp1 = new DefaultValueComponent1 { Str1 = null, Comp2 = null } };
			var expectedVer2 = new DefaultValueComponentTestEntity { Id = id6, Comp1 = new DefaultValueComponent1 { Str1 = null, Comp2 = new DefaultValueComponent2 { Str1 = "upd-c2-str1", Str2 = null } } };
			ent1.Should().Be.EqualTo(expectedVer1);
			ent2.Should().Be.EqualTo(expectedVer2);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Components.Mapping.hbm.xml" };
			}
		}
	}
}