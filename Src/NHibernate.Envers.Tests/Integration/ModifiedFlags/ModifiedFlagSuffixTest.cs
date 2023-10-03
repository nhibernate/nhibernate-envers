using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Integration.Basic;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class ModifiedFlagSuffixTest : AbstractModifiedFlagsEntityTest
	{
		private int id;

		public ModifiedFlagSuffixTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			base.AddToConfiguration(configuration);
			configuration.SetEnversProperty(ConfigurationKey.ModifiedFlagSuffix, "_CHANGED");
		}

		protected override void Initialize()
		{
			using (var tx = Session.BeginTransaction())
			{
				var entity = new BasicTestEntity1 {Str1 = "x", Long1 = 1};
				id = (int) Session.Save(entity);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyModFlagProperties()
		{
			Cfg.GetClassMapping("NHibernate.Envers.Tests.Integration.Basic.BasicTestEntity1_AUD")
				.ExtractModProperties("_CHANGED")
				.Should().Have.SameValuesAs("Str1_CHANGED", "Long1_CHANGED");
		}

		[Test]
		public void VerifyHasChanged()
		{
			QueryForPropertyHasChangedWithDeleted(typeof (BasicTestEntity1), id, "Str1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1);
			QueryForPropertyHasChangedWithDeleted(typeof(BasicTestEntity1), id, "Long1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1);
			AuditReader().CreateQuery().ForRevisionsOfEntity(typeof (BasicTestEntity1), false, true)
				.Add(AuditEntity.Property("Str1").HasChanged())
				.Add(AuditEntity.Property("Long1").HasChanged())
				.GetResultList()
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Integration.Basic.Mapping.hbm.xml" };
			}
		}
	}
}