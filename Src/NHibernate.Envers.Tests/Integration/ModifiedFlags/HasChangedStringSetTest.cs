using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Collection;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class HasChangedStringSetTest : AbstractModifiedFlagsEntityTest
	{
		private int sse1_id;
		private int sse2_id;

		public HasChangedStringSetTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var sse1 = new StringSetEntity();
			var sse2 = new StringSetEntity();

			// Revision 1 (sse1: initialy empty, sse2: initialy 2 elements)
			using (var tx = Session.BeginTransaction())
			{
				sse2.Strings.Add("sse2_string1");
				sse2.Strings.Add("sse2_string2");
				sse1_id = (int) Session.Save(sse1);
				sse2_id = (int) Session.Save(sse2);
				tx.Commit();
			}
			// Revision 2 (sse1: adding 2 elements, sse2: adding an existing element)
			using (var tx = Session.BeginTransaction())
			{
				sse1.Strings.Add("sse1_string1");
				sse1.Strings.Add("sse1_string2");
				sse2.Strings.Add("sse2_string1");
				tx.Commit();
			}
			// Revision 3 (sse1: removing a non-existing element, sse2: removing one element)
			using (var tx = Session.BeginTransaction())
			{
				sse1.Strings.Remove("sse1_string3");
				sse2.Strings.Remove("sse2_string1");
				tx.Commit();
			}
		}

		[Test]
		public void VerifyHasChanged()
		{
			QueryForPropertyHasChanged(typeof(StringSetEntity), sse1_id, "Strings")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 2);
			QueryForPropertyHasChanged(typeof(StringSetEntity), sse2_id, "Strings")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 3);
			QueryForPropertyHasNotChanged(typeof (StringSetEntity), sse1_id, "Strings")
				.ExtractRevisionNumbersFromRevision()
				.Should().Be.Empty();
			QueryForPropertyHasNotChanged(typeof(StringSetEntity), sse2_id, "Strings")
				.ExtractRevisionNumbersFromRevision()
				.Should().Be.Empty();
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