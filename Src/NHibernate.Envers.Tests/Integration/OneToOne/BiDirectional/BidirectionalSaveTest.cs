using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToOne.BiDirectional
{
	public class BidirectionalSaveTest : TestBase
	{
		public BidirectionalSaveTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.GlobalWithModifiedFlag, true);
		}

		protected override void Initialize()
		{
		}

		[Test]
		public void TwoSaveInOneSession_NotThrowException()
		{
			var entity = new BiRefEdEntity { Id = 1, Data = "data_ed_1" };

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(entity);

				entity.Data = entity.Data + "NextValue";
				Session.Save(entity);
				tx.Commit();
			}
		}
	}
}