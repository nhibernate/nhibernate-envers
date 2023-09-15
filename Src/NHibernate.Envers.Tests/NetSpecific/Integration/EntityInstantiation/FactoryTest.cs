using NUnit.Framework;
using NHibernate.Envers.Configuration;
using NHibernate.Cfg;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.EntityInstantiation
{
	public partial class FactoryTest : TestBase
	{
		int id1;
		int id2;


		public FactoryTest(AuditStrategyForTest strategyType)	: base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ent1 = new FactoryCreatedTestEntity { StringValue = "A" };
			var ent2 = new TestEntityWithContext { StringValue = "A" };
			using (var tx = Session.BeginTransaction())
			{
				id1 = (int)Session.Save(ent1);
				id2 = (int)Session.Save(ent2);
				tx.Commit();
			}

			using (var tx = Session.BeginTransaction())
			{
				ent1.StringValue = "B";
				ent2.StringValue = "B";
				tx.Commit();
			}
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.PostInstantiationListener, typeof(PostInstantiationListener));
			base.AddToConfiguration(configuration);
		}

		[Test]
		public void VerifyCreatedByFactory()
		{
			var ent = AuditReader().Find<FactoryCreatedTestEntity>(id1, 1);
			Assert.IsTrue(ent.CreatedByFactory);
		}

		[Test]
		public void VerifyNotCreatedByFactory()
		{
			var ent = AuditReader().Find<TestEntityWithContext>(id2, 1);
			Assert.IsFalse(ent.CreatedByFactory);
		}

		[Test]
		public void VerifyContextAssignedByListener()
		{
			var ent1 = AuditReader().Find<FactoryCreatedTestEntity>(id1, 1);
			var ent2 = AuditReader().Find<TestEntityWithContext>(id2, 1);

			Assert.IsNotNull(ent1.Context);
			Assert.IsNotNull(ent2.Context);
		}
	}
}
