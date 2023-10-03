using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Collection;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class HasChangedStringMapTest : AbstractModifiedFlagsEntityTest
	{
		private int sme1_id;
		private int sme2_id;

		public HasChangedStringMapTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var sme1 = new StringMapEntity();
			var sme2 = new StringMapEntity();

			// Revision 1 (sme1: initialy empty, sme2: initialy 1 mapping)
			using (var tx = Session.BeginTransaction())
			{
				sme2.Strings["1"] = "a";
				sme1_id = (int) Session.Save(sme1);
				sme2_id = (int) Session.Save(sme2);
				tx.Commit();
			}
			// Revision 2 (sme1: adding 2 mappings, sme2: no changes)
			using (var tx = Session.BeginTransaction())
			{
				sme1.Strings["1"] = "a";
				sme1.Strings["2"] = "b";
				tx.Commit();
			}
			// Revision 3 (sme1: removing an existing mapping, sme2: replacing a value)
			using (var tx = Session.BeginTransaction())
			{
				sme1.Strings.Remove("1");
				sme2.Strings["1"] = "b";
				tx.Commit();
			}
			// No revision (sme1: removing a non-existing mapping, sme2: replacing with the same value)
			using (var tx = Session.BeginTransaction())
			{
				sme1.Strings.Remove("3");
				sme2.Strings["1"] = "b";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyHasChanged()
		{
			QueryForPropertyHasChanged(typeof (StringMapEntity), sme1_id, "Strings")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 2, 3);
			QueryForPropertyHasChanged(typeof(StringMapEntity), sme2_id, "Strings")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 3);
			QueryForPropertyHasNotChanged(typeof (StringMapEntity), sme1_id, "Strings")
				.ExtractRevisionNumbersFromRevision()
				.Should().Be.Empty();
			QueryForPropertyHasNotChanged(typeof (StringMapEntity), sme2_id, "Strings")
				.ExtractRevisionNumbersFromRevision()
				.Should().Be.Empty(); // in rev 2 there was no version generated for sme2_id
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Collection.Mapping.hbm.xml" };
			}
		}
	}
}