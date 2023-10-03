using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Naming
{
	public partial class BasicNamingTest : TestBase
	{
		private int id1;
		private int id2;

		public BasicNamingTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var nte1 = new NamingTestEntity1 {Data = "data1"};
			var nte2 = new NamingTestEntity1 {Data = "data2"};

			using (var tx = Session.BeginTransaction())
			{
				id1 = (int) Session.Save(nte1);
				id2 = (int) Session.Save(nte2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				nte1.Data = "data1'";
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				nte2.Data = "data2'";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(NamingTestEntity1), id1));
			CollectionAssert.AreEquivalent(new[] { 1, 3 }, AuditReader().GetRevisions(typeof(NamingTestEntity1), id2));
		}

		[Test]
		public void VerifyHistoryOfId1()
		{
			var ver1 = new NamingTestEntity1 {Id = id1, Data = "data1"};
			var ver2 = new NamingTestEntity1 {Id = id1, Data = "data1'"};

			Assert.AreEqual(ver1, AuditReader().Find<NamingTestEntity1>(id1, 1));
			Assert.AreEqual(ver2, AuditReader().Find<NamingTestEntity1>(id1, 2));
			Assert.AreEqual(ver2, AuditReader().Find<NamingTestEntity1>(id1, 3));
		}

		[Test]
		public void VerifyHistoryOfId2()
		{
			var ver1 = new NamingTestEntity1 { Id = id2, Data = "data2" };
			var ver2 = new NamingTestEntity1 { Id = id2, Data = "data2'" };

			Assert.AreEqual(ver1, AuditReader().Find<NamingTestEntity1>(id2, 1));
			Assert.AreEqual(ver1, AuditReader().Find<NamingTestEntity1>(id2, 2));
			Assert.AreEqual(ver2, AuditReader().Find<NamingTestEntity1>(id2, 3));
		}

		[Test]
		public void VerifyTableName()
		{
			var auditName = TestAssembly + ".Integration.Naming.NamingTestEntity1_AUD";
			Assert.AreEqual("naming_test_entity_1_versions", Cfg.GetClassMapping(auditName).Table.Name);
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