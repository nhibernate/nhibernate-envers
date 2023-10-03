using System.Linq;
using NHibernate.Mapping;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Naming.Ids
{
	public partial class JoinEmbIdNamingTest : TestBase
	{
		private EmbIdNaming ed_id1;
		private EmbIdNaming ed_id2;
		private EmbIdNaming ing_id1;

		public JoinEmbIdNamingTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			ed_id1 = new EmbIdNaming { X = 10, Y = 20 };
			ed_id2 = new EmbIdNaming { X = 11, Y = 21 };
			ing_id1 = new EmbIdNaming { X = 12, Y = 22 };
			var ed1 = new JoinEmbIdNamingRefEdEntity { Id = ed_id1, Data = "data1" };
			var ed2 = new JoinEmbIdNamingRefEdEntity { Id = ed_id2, Data = "data2" };
			var ing1 = new JoinEmbIdNamingRefIngEntity { Id = ing_id1, Data = "x", Reference = ed1 };

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ed1);
				Session.Save(ed2);
				Session.Save(ing1);
				tx.Commit();
			}

			using (var tx = Session.BeginTransaction())
			{
				ing1.Data = "y";
				ing1.Reference = ed2;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(JoinEmbIdNamingRefEdEntity), ed_id1));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(JoinEmbIdNamingRefEdEntity), ed_id2));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(JoinEmbIdNamingRefIngEntity), ing_id1));
		}

		[Test]
		public void VerifyHistoryOfEd1()
		{
			var ver1 = new JoinEmbIdNamingRefEdEntity { Id = ed_id1, Data = "data1" };

			var rev1 = AuditReader().Find<JoinEmbIdNamingRefEdEntity>(ed_id1, 1);
			var rev2 = AuditReader().Find<JoinEmbIdNamingRefEdEntity>(ed_id1, 2);

			Assert.AreEqual(ver1, rev1);
			Assert.AreEqual(ver1, rev2);
		}

		[Test]
		public void VerifyHistoryOfEd2()
		{
			var ver1 = new JoinEmbIdNamingRefEdEntity { Id = ed_id2, Data = "data2" };

			var rev1 = AuditReader().Find<JoinEmbIdNamingRefEdEntity>(ed_id2, 1);
			var rev2 = AuditReader().Find<JoinEmbIdNamingRefEdEntity>(ed_id2, 2);

			Assert.AreEqual(ver1, rev1);
			Assert.AreEqual(ver1, rev2);
		}

		[Test]
		public void VerifyHistoryOfIng1()
		{
			var ver1 = new JoinEmbIdNamingRefIngEntity { Id = ing_id1, Data = "x", Reference = null };
			var ver2 = new JoinEmbIdNamingRefIngEntity { Id = ing_id1, Data = "y", Reference = null };

			var rev1 = AuditReader().Find<JoinEmbIdNamingRefIngEntity>(ing_id1, 1);
			var rev2 = AuditReader().Find<JoinEmbIdNamingRefIngEntity>(ing_id1, 2);

			Assert.AreEqual(ver1, rev1);
			Assert.AreEqual(ver2, rev2);

			Assert.AreEqual(new JoinEmbIdNamingRefEdEntity { Id = ed_id1, Data = "data1" }, rev1.Reference);
			Assert.AreEqual(new JoinEmbIdNamingRefEdEntity { Id = ed_id2, Data = "data2" }, rev2.Reference);
		}

		[Test]
		public void VerifyColumnNames()
		{
			var auditName = TestAssembly + ".Integration.Naming.Ids.JoinEmbIdNamingRefIngEntity_AUD";
			var xcolumns = Cfg.GetClassMapping(auditName).GetProperty("Reference_X").ColumnIterator;
			xcolumns.Should().Have.Count.EqualTo(1);
			((Column) xcolumns.First()).Name.Should().Be.EqualTo("XX_Reference");

			var ycolumns = Cfg.GetClassMapping(auditName).GetProperty("Reference_Y").ColumnIterator;
			ycolumns.Should().Have.Count.EqualTo(1);
			((Column)ycolumns.First()).Name.Should().Be.EqualTo("YY_Reference");
		}
	}
}