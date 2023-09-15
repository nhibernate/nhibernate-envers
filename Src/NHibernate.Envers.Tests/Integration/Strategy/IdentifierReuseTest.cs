using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tests.Entities.ManyToMany;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Strategy
{
	public partial class IdentifierReuseTest :TestBase
	{
		private const int reusedId = 11;

		public IdentifierReuseTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			saveUpdateAndRemoveEntity();
			saveUpdateAndRemoveEntity();
		}

		[Test]
		public void VerifyIdentifierReuse()
		{
			AuditReader().GetRevisions(typeof (IntNoAutoIdTestEntity), reusedId)
				.Should().Have.SameSequenceAs(1, 2, 3, 4, 5, 6);
		}

		private void saveUpdateAndRemoveEntity()
		{
			var entity = new IntNoAutoIdTestEntity { Id = reusedId, Number = 0 };
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(entity);
				tx.Commit();
			}

			using (var tx = Session.BeginTransaction())
			{
				entity.Number = 1;
				tx.Commit();
			}

			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(entity);
				tx.Commit();
			}
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.AllowIdentifierReuse, true);
		}

		protected override System.Collections.Generic.IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.ManyToMany.Mapping.hbm.xml" };
			}
		}
	}
}