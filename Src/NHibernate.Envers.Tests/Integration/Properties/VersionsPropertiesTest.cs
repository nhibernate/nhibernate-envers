using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Properties
{
	public partial class VersionsPropertiesTest : TestBase
	{
		private int id;

		public VersionsPropertiesTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.AuditTablePrefix, "VP_");
			configuration.SetEnversProperty(ConfigurationKey.AuditTableSuffix, "_VS");
			configuration.SetEnversProperty(ConfigurationKey.RevisionFieldName, "ver_rev");
			configuration.SetEnversProperty(ConfigurationKey.RevisionTypeFieldName, "ver_rev_type");
		}

		protected override void Initialize()
		{
			var pte = new PropertiesTestEntity { Str = "x" };
			using (var tx = Session.BeginTransaction())
			{
				id = (int) Session.Save(pte);
				tx.Commit();
			} 
			using (var tx = Session.BeginTransaction())
			{
				pte.Str = "y";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			AuditReader().GetRevisions(typeof(PropertiesTestEntity), id)
				.Should().Have.SameSequenceAs(1, 2);
		}

		[Test]
		public void VerifyHistory()
		{
			var ver1 = new PropertiesTestEntity { Id = id, Str = "x" };
			var ver2 = new PropertiesTestEntity { Id = id, Str = "y" };

			AuditReader().Find<PropertiesTestEntity>(id, 1)
				.Should().Be.EqualTo(ver1);
			AuditReader().Find<PropertiesTestEntity>(id, 2)
				.Should().Be.EqualTo(ver2);
		}

		[Test]
		public void ShouldHaveCorrectSchema()
		{
			var table = Cfg.GetClassMapping("VP_" + typeof (PropertiesTestEntity).FullName + "_VS").Table;
			var containsRevColumnName = false;
			var containsRevTypeColumnName = false;
			foreach (var column in table.ColumnIterator)
			{
				if (column.Name.Equals("ver_rev"))
					containsRevColumnName = true;
				if (column.Name.Equals("ver_rev_type"))
					containsRevTypeColumnName = true;
			}
			containsRevColumnName.Should().Be.True();
			containsRevTypeColumnName.Should().Be.True();
		}
	}
}