using System;
using NHibernate.Envers.Exceptions;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Basic
{
	public partial class SingleOperationInTransactionTest : TestBase
	{
		private int id1;
		private int id2;
		private int id3;

		public SingleOperationInTransactionTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		private int addNewEntity(string str, long lng)
		{
			using (var tx = Session.BeginTransaction())
			{
				var bte = new BasicTestEntity1 { Str1 = str, Long1 = lng };
				Session.Save(bte);
				tx.Commit();
				return bte.Id;
			}
		}

		private void modifyEntity(int id, string str, long lng)
		{
			using (var tx = Session.BeginTransaction())
			{
				var bte = Session.Get<BasicTestEntity1>(id);
				bte.Long1 = lng;
				bte.Str1 = str;
				tx.Commit();
			}
		}

		protected override void Initialize()
		{
			id1 = addNewEntity("x", 1); // rev 1
			id2 = addNewEntity("y", 20); // rev 2
			id3 = addNewEntity("z", 30); // rev 3

			modifyEntity(id1, "x2", 2); // rev 4
			modifyEntity(id2, "y2", 20); // rev 5
			modifyEntity(id1, "x3", 3); // rev 6
			modifyEntity(id1, "x3", 3); // no rev
			modifyEntity(id2, "y3", 21); // rev 7
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 4, 6 }, AuditReader().GetRevisions(typeof(BasicTestEntity1), id1));
			CollectionAssert.AreEquivalent(new[] { 2, 5, 7 }, AuditReader().GetRevisions(typeof(BasicTestEntity1), id2));
			CollectionAssert.AreEquivalent(new[] { 3 }, AuditReader().GetRevisions(typeof(BasicTestEntity1), id3));
		}

		[Test]
		public void VerifyRevisionDates()
		{
			for (var i=1; i<7; i++)
			{
				Assert.GreaterOrEqual(AuditReader().GetRevisionDate(i + 1), AuditReader().GetRevisionDate(i));
			}
		}

		[Test]
		public void NotExistingRevisionShouldThrow()
		{
			Assert.Throws<RevisionDoesNotExistException>(() =>
									AuditReader().GetRevisionDate(8)
						);
		}

		[Test]
		public void IllegalRevisionShouldThrow()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() =>
				AuditReader().GetRevisionDate(0)
						 );
		}

		[Test]
		public void VerifyHistoryOf1()
		{
			var ver1 = new BasicTestEntity1 { Id = id1, Str1 = "x", Long1 = 1 };
			var ver2 = new BasicTestEntity1 { Id = id1, Str1 = "x2", Long1 = 2 };
			var ver3 = new BasicTestEntity1 { Id = id1, Str1 = "x3", Long1 = 3 };

			Assert.AreEqual(ver1, AuditReader().Find<BasicTestEntity1>(id1, 1));
			Assert.AreEqual(ver1, AuditReader().Find<BasicTestEntity1>(id1, 2));
			Assert.AreEqual(ver1, AuditReader().Find<BasicTestEntity1>(id1, 3));
			Assert.AreEqual(ver2, AuditReader().Find<BasicTestEntity1>(id1, 4));
			Assert.AreEqual(ver2, AuditReader().Find<BasicTestEntity1>(id1, 5));
			Assert.AreEqual(ver3, AuditReader().Find<BasicTestEntity1>(id1, 6));
			Assert.AreEqual(ver3, AuditReader().Find<BasicTestEntity1>(id1, 7));
		}

		[Test]
		public void VerifyHistoryOf2()
		{
			var ver1 = new BasicTestEntity1 { Id = id2, Str1 = "y", Long1 = 20 };
			var ver2 = new BasicTestEntity1 { Id = id2, Str1 = "y2", Long1 = 20 };
			var ver3 = new BasicTestEntity1 { Id = id2, Str1 = "y3", Long1 = 21 };

			Assert.IsNull(AuditReader().Find<BasicTestEntity1>(id2, 1));
			Assert.AreEqual(ver1, AuditReader().Find<BasicTestEntity1>(id2, 2));
			Assert.AreEqual(ver1, AuditReader().Find<BasicTestEntity1>(id2, 3));
			Assert.AreEqual(ver1, AuditReader().Find<BasicTestEntity1>(id2, 4));
			Assert.AreEqual(ver2, AuditReader().Find<BasicTestEntity1>(id2, 5));
			Assert.AreEqual(ver2, AuditReader().Find<BasicTestEntity1>(id2, 6));
			Assert.AreEqual(ver3, AuditReader().Find<BasicTestEntity1>(id2, 7));
		}

		[Test]
		public void VerifyHistoryOf3()
		{
			var ver1 = new BasicTestEntity1 { Id = id3, Str1 = "z", Long1 = 30 };

			Assert.IsNull(AuditReader().Find<BasicTestEntity1>(id3, 1));
			Assert.IsNull(AuditReader().Find<BasicTestEntity1>(id3, 2));
			Assert.AreEqual(ver1, AuditReader().Find<BasicTestEntity1>(id3, 3));
			Assert.AreEqual(ver1, AuditReader().Find<BasicTestEntity1>(id3, 4));
			Assert.AreEqual(ver1, AuditReader().Find<BasicTestEntity1>(id3, 5));
			Assert.AreEqual(ver1, AuditReader().Find<BasicTestEntity1>(id3, 6));
			Assert.AreEqual(ver1, AuditReader().Find<BasicTestEntity1>(id3, 7));
		}

		[Test]
		public void VerifyHistoryOfNonExistingEntity()
		{
			Assert.IsNull(AuditReader().Find<BasicTestEntity1>(id1 + id2 + id3, 1));
			Assert.IsNull(AuditReader().Find<BasicTestEntity1>(id1 + id2 + id3, 7));
		}
	}
}