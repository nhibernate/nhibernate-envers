using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.AccessType
{
	public partial class FieldAccessTest : TestBase
	{
		private int id;

		public FieldAccessTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var fa = new FieldAccessEntity();
			fa.SetData("first");
			using (var tx = Session.BeginTransaction())
			{
				id = (int)Session.Save(fa);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				fa.SetData("second");
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(FieldAccessEntity), id));
		}

		[Test]
		public void VerifyHistory()
		{
			var ver1 = new FieldAccessEntity { Id = id };
			ver1.SetData("first");
			var ver2 = new FieldAccessEntity { Id = id };
			ver2.SetData("second");

			Assert.AreEqual(ver1, AuditReader().Find<FieldAccessEntity>(id, 1));
			Assert.AreEqual(ver2, AuditReader().Find<FieldAccessEntity>(id, 2));
		}
	}
}