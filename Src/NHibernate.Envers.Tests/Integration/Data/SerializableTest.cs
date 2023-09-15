using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Data
{
	public partial class SerializableTest : TestBase
	{
		private int id1;

		public SerializableTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ste = new SerializableTestEntity { Obj = new SerObj { Data = "d1" } };
			using (var tx = Session.BeginTransaction())
			{
				id1 = (int) Session.Save(ste);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ste.Obj = new SerObj {Data = "d2"};
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(SerializableTestEntity), id1));
		}

		[Test]
		public void VerifyHistoryOfId1()
		{
			var ver1 = new SerializableTestEntity { Id = id1, Obj = new SerObj { Data = "d1" } };
			var ver2 = new SerializableTestEntity { Id = id1, Obj = new SerObj { Data = "d2" } };

			Assert.AreEqual(ver1, AuditReader().Find<SerializableTestEntity>(id1, 1));
			Assert.AreEqual(ver2, AuditReader().Find<SerializableTestEntity>(id1, 2));
		}
	}
}