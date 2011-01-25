using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Ids;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Ids
{
	[TestFixture]
	public class CompositeIdTest : TestBase
	{
		private EmbId id1;
		private EmbId id2;
		private MulId id3;
		private MulId id4;

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Ids.Mapping.hbm.xml" }; }
		}


		protected override void Initialize()
		{
			id1 = new EmbId { X = 1, Y = 2 };
			id2 = new EmbId { X = 10, Y = 20 };
			id3 = new MulId { Id1 = 100, Id2 = 101 };
			id4 = new MulId { Id1 = 102, Id2 = 103 };

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(new EmbIdTestEntity {Id = id1, Str1 = "x"});
				//Session.Save(new MulIdTestEntity {Id1 = id3.Id1, Id2 = id3.Id2, Str1 = "a"});
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(new EmbIdTestEntity { Id = id2, Str1 = "y" });
				//Session.Save(new MulIdTestEntity { Id1 = id4.Id1, Id2 = id4.Id2, Str1 = "b" });
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				var ete1 = Session.Get<EmbIdTestEntity>(id1);
				var ete2 = Session.Get<EmbIdTestEntity>(id2);
				//var mte3 = Session.Get<MulIdTestEntity>(id3);
				//var mte4 = Session.Get<MulIdTestEntity>(id4);
				ete1.Str1 = "x2";
				ete2.Str1 = "y2";
				//mte3.Str1 = "a2";
				//mte4.Str1 = "b2";
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				var ete1 = Session.Load<EmbIdTestEntity>(id1);
				var ete2 = Session.Load<EmbIdTestEntity>(id2);
				//var mte3 = Session.Load<MulIdTestEntity>(id3);
				Session.Delete(ete1);
				//Session.Delete(mte3);
				ete2.Str1 = "y3";
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
			//CollectionAssert.AreEquivalent(new[] { 1, 3, 4 }, AuditReader.GetRevisions(typeof(MulIdTestEntity), id3));
			//CollectionAssert.AreEquivalent(new[] { 2, 3 }, AuditReader.GetRevisions(typeof(MulIdTestEntity), id4));
		}


		[Test]
		public void VerifyHistoryOfId1()
		{
			var ver1 = new EmbIdTestEntity {Id = id1, Str1 = "x"};
			var ver2 = new EmbIdTestEntity {Id = id1, Str1 = "x2"};

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

		[Test, Ignore("Multiple id (mapped=true) is not supported in NH Core")]
		public void VerifyHistoryOfId3(){}


		[Test, Ignore("Multiple id (mapped=true) is not supported in NH Core")]
		public void VerifyHistoryOfId4() { }
	}
}