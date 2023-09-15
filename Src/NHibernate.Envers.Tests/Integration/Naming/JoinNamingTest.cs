using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Naming
{
	public partial class JoinNamingTest : TestBase
	{
		private int ed_id1;
		private int ed_id2;
		private int ing_id1;

		public JoinNamingTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ed1 = new JoinNamingRefEdEntity {Data = "data1"};
			var ed2 = new JoinNamingRefEdEntity {Data = "data2"};
			var ing1 = new JoinNamingRefIngEntity {Data = "x", Reference = ed1};
			using (var tx = Session.BeginTransaction())
			{
				ed_id1 = (int) Session.Save(ed1);
				ed_id2 = (int) Session.Save(ed2);
				ing_id1 = (int) Session.Save(ing1);
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
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(JoinNamingRefEdEntity), ed_id1));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(JoinNamingRefEdEntity), ed_id2));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(JoinNamingRefIngEntity), ing_id1));
		}

		[Test]
		public void VerifyHistoryOfEdId1()
		{
			var ver1 = new JoinNamingRefEdEntity {Id = ed_id1, Data = "data1"};

			Assert.AreEqual(ver1, AuditReader().Find<JoinNamingRefEdEntity>(ed_id1, 1));
			Assert.AreEqual(ver1, AuditReader().Find<JoinNamingRefEdEntity>(ed_id1, 2));
		}

		[Test]
		public void VerifyHistoryOfEdId2()
		{
			var ver1 = new JoinNamingRefEdEntity { Id = ed_id2, Data = "data2" };

			Assert.AreEqual(ver1, AuditReader().Find<JoinNamingRefEdEntity>(ed_id2, 1));
			Assert.AreEqual(ver1, AuditReader().Find<JoinNamingRefEdEntity>(ed_id2, 2));
		}

		[Test]
		public void VerifyHistoryOfIngId1()
		{
			var ver1 = new JoinNamingRefIngEntity {Id = ing_id1, Data = "x", Reference = null};
			var ver2 = new JoinNamingRefIngEntity {Id = ing_id1, Data = "y", Reference = null};

			Assert.AreEqual(ver1, AuditReader().Find<JoinNamingRefIngEntity>(ing_id1, 1));
			Assert.AreEqual(ver2, AuditReader().Find<JoinNamingRefIngEntity>(ing_id1, 2));

			Assert.AreEqual(new JoinNamingRefEdEntity { Id = ed_id1, Data = "data1" },
							AuditReader().Find<JoinNamingRefIngEntity>(ing_id1, 1).Reference);
			Assert.AreEqual(new JoinNamingRefEdEntity { Id = ed_id2, Data = "data2" },
										AuditReader().Find<JoinNamingRefIngEntity>(ing_id1, 2).Reference);
		}

		[Test]
		public void VerifyJoinColumnName()
		{
			var auditName = TestAssembly + ".Integration.Naming.JoinNamingRefIngEntity_AUD";
			var columns = Cfg.GetClassMapping(auditName).GetProperty("Reference_Id").ColumnIterator;
			Assert.AreEqual(1, columns.Count());
			Assert.AreEqual("jnree_column_reference", ((Column)columns.First()).Name);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Integration.Naming.Mapping.hbm.xml", "Entities.Mapping.hbm.xml" };
			}
		}
	}
}