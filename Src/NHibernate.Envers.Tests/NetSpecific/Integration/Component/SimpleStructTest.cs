using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Component
{
	public partial class SimpleStructTest : TestBase
	{
		private const int id = 111;

		public SimpleStructTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var owner = new ComponentOwner { Id = id };

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(owner);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				var comp = new StructComponent {Data1 = "1", NestedComponent = new StructComponent2{Data2=1}};
				owner.Component = comp;
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				var comp = new StructComponent { Data1 = "2", NestedComponent = new StructComponent2{Data2=2} };
				owner.Component = comp;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			CollectionAssert.AreEqual(new[] {1, 2, 3}, AuditReader().GetRevisions(typeof(ComponentOwner),id));
		}

		[Test]
		public void VerifyHistoryOfComponent()
		{
			var ver1 = new StructComponent();
			var ver2 = new StructComponent { Data1 = "1", NestedComponent = new StructComponent2 { Data2 = 1 } };
			var ver3 = new StructComponent { Data1 = "2", NestedComponent = new StructComponent2 { Data2 = 2 } };


			Assert.AreEqual(ver1, AuditReader().Find<ComponentOwner>(id, 1).Component);
			Assert.AreEqual(ver2, AuditReader().Find<ComponentOwner>(id, 2).Component);
			Assert.AreEqual(ver3, AuditReader().Find<ComponentOwner>(id, 3).Component);
		}
	}
}