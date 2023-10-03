using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Collection;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class HasChangedEnumSetTest : AbstractModifiedFlagsEntityTest
	{
		private int id;

		public HasChangedEnumSetTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var sse1 = new EnumSetEntity();

			// Revision 1 (sse1: initialy 1 element)
			using (var tx = Session.BeginTransaction())
			{
				sse1.Enums1.Add(E1.X);
				sse1.Enums2.Add(E2.A);
				id = (int) Session.Save(sse1);
				tx.Commit();
			}

			// Revision 2 (sse1: adding 1 element/removing a non-existing element)
			using (var tx = Session.BeginTransaction())
			{
				sse1.Enums1.Add(E1.Y);
				sse1.Enums2.Remove(E2.B);
				tx.Commit();
			}

			// Revision 3 (sse1: removing 1 element/adding an exisiting element)
			using (var tx = Session.BeginTransaction())
			{
				sse1.Enums1.Remove(E1.X);
				sse1.Enums2.Add(E2.A);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyHasChanged()
		{
			QueryForPropertyHasChanged(typeof (EnumSetEntity), id, "Enums1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 2, 3);
			QueryForPropertyHasChanged(typeof(EnumSetEntity), id, "Enums2")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1);
			QueryForPropertyHasNotChanged(typeof (EnumSetEntity), id, "Enums1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Be.Empty();
			QueryForPropertyHasNotChanged(typeof(EnumSetEntity), id, "Enums2")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(2, 3);

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