using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.OneToOne.BiDirectional
{
	public partial class BidirectionalSaveTest : TestBase
	{
		private const int id = 18;

		public BidirectionalSaveTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.GlobalWithModifiedFlag, true);
		} 
		

		protected override void Initialize()
		{
			var entity = new BiRefEdEntity { Id = id, Data = "1" };

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(entity);

				entity.Data = "2";
				Session.Update(entity);
				tx.Commit();
			}
		}

		
		[Test]
		public void VerifyRevisionCount()
		{
			AuditReader().GetRevisions(typeof(BiRefEdEntity), id)
				.Should().Have.SameSequenceAs(1);
		}
		
		[Test]
		public void VerifyHistory()
		{
			AuditReader().Find<BiRefEdEntity>(id, 1).Data.Should().Be.EqualTo("2");
		}
	}
}