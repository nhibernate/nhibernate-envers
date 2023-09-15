using System.Collections.Generic;
using NHibernate.Envers.Tests.Integration.Basic;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class HasChangedInsertUpdateSameTransactionTest : AbstractModifiedFlagsEntityTest
	{
		public HasChangedInsertUpdateSameTransactionTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			using (var tx = Session.BeginTransaction())
			{
				var entity = new BasicTestEntity1 {Long1 = 1, Str1 = "str1"};
				Session.Save(entity);
				entity.Str1 = "str2";
				Session.Merge(entity);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyPropertyChangedInsertUpdateSameTransaction()
		{
			QueryForPropertyHasChanged(typeof(BasicTestEntity1), 1, "Long1").Count
				.Should().Be.EqualTo(1);
		}

		protected override IEnumerable<string> Mappings => new[] { "Integration.Basic.Mapping.hbm.xml" };
	}
}