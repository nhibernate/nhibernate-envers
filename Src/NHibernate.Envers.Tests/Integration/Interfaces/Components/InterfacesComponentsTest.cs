using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Interfaces.Components
{
	public partial class InterfacesComponentsTest : TestBase
	{
		private int id1;

		public InterfacesComponentsTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var cte1 = new ComponentTestEntity { Comp1 = new Component1 { Data = "a" } };
			using (var tx = Session.BeginTransaction())
			{
				id1 = (int)Session.Save(cte1);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				cte1.Comp1 = new Component1 { Data = "b" };
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				cte1.Comp1.Data = "c";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, AuditReader().GetRevisions(typeof(ComponentTestEntity), id1));
		}

		[Test]
		public void VerifyHistory()
		{
			var ver1 = new ComponentTestEntity { Id = id1, Comp1 = new Component1 { Data = "a" } };
			var ver2 = new ComponentTestEntity { Id = id1, Comp1 = new Component1 { Data = "b" } };
			var ver3 = new ComponentTestEntity { Id = id1, Comp1 = new Component1 { Data = "c" } };

			Assert.AreEqual(ver1, AuditReader().Find<ComponentTestEntity>(id1, 1));
			Assert.AreEqual(ver2, AuditReader().Find<ComponentTestEntity>(id1, 2));
			Assert.AreEqual(ver3, AuditReader().Find<ComponentTestEntity>(id1, 3));
		}
	}
}