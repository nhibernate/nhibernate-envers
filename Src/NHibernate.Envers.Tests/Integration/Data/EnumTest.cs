using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Data
{
	public partial class EnumTest : TestBase
	{
		private int id1;

		public EnumTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ete = new EnumTestEntity { Enum1 = E1.X, Enum2 = E2.A };
			using (var tx = Session.BeginTransaction())
			{
				id1 = (int)Session.Save(ete);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				ete.Enum1 = E1.Y;
				ete.Enum2 = E2.B;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(EnumTestEntity), id1));
		}

		[Test]
		public void VerifyHistoryOfId1()
		{
			var ver1 = new EnumTestEntity {Id = id1, Enum1 = E1.X, Enum2 = E2.A};
			var ver2 = new EnumTestEntity {Id = id1, Enum1 = E1.Y, Enum2 = E2.B};

			Assert.AreEqual(ver1, AuditReader().Find<EnumTestEntity>(id1, 1));
			Assert.AreEqual(ver2, AuditReader().Find<EnumTestEntity>(id1, 2));
		}
	}
}