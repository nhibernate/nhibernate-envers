using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Properties
{
	public partial class UnversionedOptimisticLockingFieldTest : TestBase
	{
		private int id;

		public UnversionedOptimisticLockingFieldTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.DoNotAuditOptimisticLockingField, true);
		}

		protected override void Initialize()
		{
			var olfe = new UnversionedOptimisticLockingFieldEntity{Str="x"};
			using (var tx = Session.BeginTransaction())
			{
				id = (int) Session.Save(olfe);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				olfe.Str = "y";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			AuditReader().GetRevisions(typeof (UnversionedOptimisticLockingFieldEntity), id)
				.Should().Have.SameSequenceAs(1, 2);
		}

		[Test]
		public void VerifyHistory()
		{
			var ver1 = new UnversionedOptimisticLockingFieldEntity {Id = id, Str = "x"};
			var ver2 = new UnversionedOptimisticLockingFieldEntity {Id = id, Str = "y"};

			AuditReader().Find<UnversionedOptimisticLockingFieldEntity>(id, 1)
				.Should().Be.EqualTo(ver1);
			AuditReader().Find<UnversionedOptimisticLockingFieldEntity>(id, 2)
				.Should().Be.EqualTo(ver2);
		}

		[Test]
		public void ShouldNotStoreVersionNumber()
		{
			var pc = Cfg.GetClassMapping(typeof (UnversionedOptimisticLockingFieldEntity).FullName + "_AUD");
			foreach (var property in pc.PropertyIterator)
			{
				property.Name.Should().Not.Be.EqualTo("OptLocking");
			}
		}
	}
}