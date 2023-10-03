using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.AccessType
{
	public partial class PropertyAccessTest : TestBase
	{
		private int id;

		public PropertyAccessTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var fa = new PropertyAccessEntity { Data = "first" };
			using (var tx = Session.BeginTransaction())
			{
				id = (int)Session.Save(fa);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				fa.Data = "second";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(PropertyAccessEntity), id));
		}

		[Test]
		public void VerifyHistory()
		{
			Assert.AreEqual("first", AuditReader().Find<PropertyAccessEntity>(id, 1).Data);
			Assert.AreEqual("second", AuditReader().Find<PropertyAccessEntity>(id, 2).Data);
		}
	}
}