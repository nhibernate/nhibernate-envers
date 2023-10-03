using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Ctor
{
	public partial class NonPublicCtorTest : TestBase
	{
		private int id;

		public NonPublicCtorTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var entity = new NonPublicCtorEntity(new NonPublicCtorComponent("first"), new StructComponentWithDefinedCtor(1));
			using (var tx = Session.BeginTransaction())
			{
				id = (int) Session.Save(entity);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				entity.Component.Name = "second";
				entity.StructComponent = new StructComponentWithDefinedCtor(2);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(NonPublicCtorEntity),id));
		}

		[Test]
		public void VerifyHistoryOfRefComponent()
		{
			AuditReader().Find<NonPublicCtorEntity>(id, 1).Component.Name
				.Should().Be.EqualTo("first");
			AuditReader().Find<NonPublicCtorEntity>(id, 2).Component.Name
				.Should().Be.EqualTo("second");
		}

		[Test]
		public void VerifyHistoryOfStructComponent()
		{
			AuditReader().Find<NonPublicCtorEntity>(id, 1).StructComponent.Value
				.Should().Be.EqualTo(1);
			AuditReader().Find<NonPublicCtorEntity>(id, 2).StructComponent.Value
				.Should().Be.EqualTo(2);
		}
	}
}