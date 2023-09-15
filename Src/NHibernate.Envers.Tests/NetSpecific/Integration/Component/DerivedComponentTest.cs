using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Component
{
	public partial class DerivedComponentTest : TestBase
	{
		private const int id = 111;

		public DerivedComponentTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var baseowner = new BaseClassOwner { Id = id, Component = new BaseClassComponent() };

			// Insert some audit data for baseowner
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(baseowner);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				baseowner.Component.Data1 = "1";
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				baseowner.Component.Data1 = "2";
				tx.Commit();
			}

			// DerivedClassComponent derives from BaseClassComponent
			// Just do the same with it like with baseowner
			var derivedowner = new DerivedClassOwner { Id = id, Component = new DerivedClassComponent() };

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(derivedowner);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				derivedowner.Component.Data1 = "1";
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				derivedowner.Component.Data1 = "2";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyActualBaseClassOwner()
		{
			var owner = Session.Get<BaseClassOwner>(id);
			Assert.That(owner.Component, Is.Not.Null);
			Assert.That(owner.Component.Data1, Is.EqualTo("2"));
		}

		[Test]
		public void VerifyHistoryOfBaseClassOwner()
		{
			var owner = AuditReader().Find<BaseClassOwner>(id, 1);
			Assert.That(owner.Component, Is.Null);
			owner = AuditReader().Find<BaseClassOwner>(id, 2);
			Assert.That(owner.Component, Is.Not.Null);
			Assert.That(owner.Component.Data1, Is.EqualTo("1"));
			owner = AuditReader().Find<BaseClassOwner>(id, 3);
			Assert.That(owner.Component, Is.Not.Null);
			Assert.That(owner.Component.Data1, Is.EqualTo("2"));
		}

		[Test]
		public void VerifyActualDerivedClassOwner()
		{
			var owner = Session.Get<DerivedClassOwner>(id);
			Assert.That(owner.Component, Is.Not.Null);
			Assert.That(owner.Component.Data1, Is.EqualTo("2"));
		}

		[Test]
		public void VerifyHistoryOfDerivedClassOwner()
		{
			var owner = AuditReader().Find<DerivedClassOwner>(id, 4);
			Assert.That(owner.Component, Is.Null);
			owner = AuditReader().Find<DerivedClassOwner>(id, 5);
			Assert.That(owner.Component, Is.Not.Null);
			Assert.That(owner.Component.Data1, Is.EqualTo("1"));
			owner = AuditReader().Find<DerivedClassOwner>(id, 6);
			Assert.That(owner.Component, Is.Not.Null);
			Assert.That(owner.Component.Data1, Is.EqualTo("2"));
		}
	}
}