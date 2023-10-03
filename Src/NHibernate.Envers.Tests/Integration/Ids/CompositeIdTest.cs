using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Ids;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Ids
{
	public partial class CompositeIdTest : TestBase
	{
		private EmbId id1;
		private EmbId id2;
		private EmbIdWithCustomType id5;
		private EmbIdWithCustomType id6;

		public CompositeIdTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Ids.Mapping.hbm.xml" }; }
		}

		protected override void Initialize()
		{
			id1 = new EmbId { X = 1, Y = 2 };
			id2 = new EmbId { X = 10, Y = 20 };
			id5 = new EmbIdWithCustomType { X = 25, CustomEnum = CustomEnum.No };
			id6 = new EmbIdWithCustomType { X = 27, CustomEnum = CustomEnum.Yes };

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(new EmbIdTestEntity { Id = id1, Str1 = "x" });
				Session.Save(new EmbIdWithCustomTypeTestEntity { Id = id5, Str1 = "c" });
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(new EmbIdTestEntity { Id = id2, Str1 = "y" });
				Session.Save(new EmbIdWithCustomTypeTestEntity { Id = id6, Str1 = "d" });
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				var ete1 = Session.Get<EmbIdTestEntity>(id1);
				var ete2 = Session.Get<EmbIdTestEntity>(id2);
				var cte5 = Session.Get<EmbIdWithCustomTypeTestEntity>(id5);
				var cte6 = Session.Get<EmbIdWithCustomTypeTestEntity>(id6);
				ete1.Str1 = "x2";
				ete2.Str1 = "y2";
				cte5.Str1 = "c2";
				cte6.Str1 = "d2";
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				var ete1 = Session.Load<EmbIdTestEntity>(id1);
				var ete2 = Session.Load<EmbIdTestEntity>(id2);
				var cte5 = Session.Get<EmbIdWithCustomTypeTestEntity>(id5);
				var cte6 = Session.Get<EmbIdWithCustomTypeTestEntity>(id6);
				Session.Delete(ete1);
				Session.Delete(cte6);
				ete2.Str1 = "y3";
				cte5.Str1 = "c3";
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				var ete2 = Session.Get<EmbIdTestEntity>(id2);
				Session.Delete(ete2);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 3, 4 }, AuditReader().GetRevisions(typeof(EmbIdTestEntity), id1));
			CollectionAssert.AreEquivalent(new[] { 2, 3, 4, 5 }, AuditReader().GetRevisions(typeof(EmbIdTestEntity), id2));

			CollectionAssert.AreEquivalent(new[] { 1, 3, 4 }, AuditReader().GetRevisions(typeof(EmbIdWithCustomTypeTestEntity), id5));
			CollectionAssert.AreEquivalent(new[] { 2, 3, 4 }, AuditReader().GetRevisions(typeof(EmbIdWithCustomTypeTestEntity), id6));
		}


		[Test]
		public void VerifyHistoryOfId1()
		{
			var ver1 = new EmbIdTestEntity { Id = id1, Str1 = "x" };
			var ver2 = new EmbIdTestEntity { Id = id1, Str1 = "x2" };

			Assert.AreEqual(ver1, AuditReader().Find<EmbIdTestEntity>(id1, 1));
			Assert.AreEqual(ver1, AuditReader().Find<EmbIdTestEntity>(id1, 2));
			Assert.AreEqual(ver2, AuditReader().Find<EmbIdTestEntity>(id1, 3));
			Assert.IsNull(AuditReader().Find<EmbIdTestEntity>(id1, 4));
			Assert.IsNull(AuditReader().Find<EmbIdTestEntity>(id1, 5));
		}


		[Test]
		public void VerifyHistoryOfId2()
		{
			var ver1 = new EmbIdTestEntity { Id = id2, Str1 = "y" };
			var ver2 = new EmbIdTestEntity { Id = id2, Str1 = "y2" };
			var ver3 = new EmbIdTestEntity { Id = id2, Str1 = "y3" };

			Assert.IsNull(AuditReader().Find<EmbIdTestEntity>(id2, 1));
			Assert.AreEqual(ver1, AuditReader().Find<EmbIdTestEntity>(id2, 2));
			Assert.AreEqual(ver2, AuditReader().Find<EmbIdTestEntity>(id2, 3));
			Assert.AreEqual(ver3, AuditReader().Find<EmbIdTestEntity>(id2, 4));
			Assert.IsNull(AuditReader().Find<EmbIdTestEntity>(id2, 5));
		}

		[Test]
		public void VerifyHistoryOfId5()
		{
			var ver1 = new EmbIdWithCustomTypeTestEntity { Id = id5, Str1 = "c" };
			var ver2 = new EmbIdWithCustomTypeTestEntity { Id = id5, Str1 = "c2" };
			var ver3 = new EmbIdWithCustomTypeTestEntity { Id = id5, Str1 = "c3" };

			AuditReader().Find<EmbIdWithCustomTypeTestEntity>(id5, 1).Should().Be.EqualTo(ver1);
			AuditReader().Find<EmbIdWithCustomTypeTestEntity>(id5, 2).Should().Be.EqualTo(ver1);
			AuditReader().Find<EmbIdWithCustomTypeTestEntity>(id5, 3).Should().Be.EqualTo(ver2);
			AuditReader().Find<EmbIdWithCustomTypeTestEntity>(id5, 4).Should().Be.EqualTo(ver3);
			AuditReader().Find<EmbIdWithCustomTypeTestEntity>(id5, 5).Should().Be.EqualTo(ver3);
		}

		[Test]
		public void VerifyHistoryOfId6()
		{
			var ver1 = new EmbIdWithCustomTypeTestEntity { Id = id6, Str1 = "d" };
			var ver2 = new EmbIdWithCustomTypeTestEntity { Id = id6, Str1 = "d2" };

			AuditReader().Find<EmbIdWithCustomTypeTestEntity>(id6, 1).Should().Be.Null();
			AuditReader().Find<EmbIdWithCustomTypeTestEntity>(id6, 2).Should().Be.EqualTo(ver1);
			AuditReader().Find<EmbIdWithCustomTypeTestEntity>(id6, 3).Should().Be.EqualTo(ver2);
			AuditReader().Find<EmbIdWithCustomTypeTestEntity>(id6, 4).Should().Be.Null();
			AuditReader().Find<EmbIdWithCustomTypeTestEntity>(id6, 5).Should().Be.Null();
		}
	}
}