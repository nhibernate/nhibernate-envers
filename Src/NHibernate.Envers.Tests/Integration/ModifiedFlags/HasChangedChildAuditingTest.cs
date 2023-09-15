using System.Collections.Generic;
using NHibernate.Envers.Tests.Integration.Inheritance.Entities;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class HasChangedChildAuditingTest : AbstractModifiedFlagsEntityTest
	{
		private int id;

		public HasChangedChildAuditingTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			id = 1;
			var ce = new ChildEntity {Id = id, Number = 11, Data = "x"};
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ce);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ce.Data = "y";
				ce.Number = 21;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyChildHasChanged()
		{
			QueryForPropertyHasChanged(typeof(ChildEntity), id, "Data")
				.ExtractRevisionNumbersFromRevision().Should().Have.SameSequenceAs(1, 2);
			QueryForPropertyHasChanged(typeof(ChildEntity), id, "Number")
				.ExtractRevisionNumbersFromRevision().Should().Have.SameSequenceAs(1, 2);
			QueryForPropertyHasNotChanged(typeof(ChildEntity), id, "Data")
				.ExtractRevisionNumbersFromRevision().Should().Be.Empty();
			QueryForPropertyHasNotChanged(typeof(ChildEntity), id, "Number")
				.ExtractRevisionNumbersFromRevision().Should().Be.Empty();
		}

		[Test]
		public void VerifyParentHasChanged()
		{
			QueryForPropertyHasChanged(typeof(ParentEntity), id, "Data")
				.ExtractRevisionNumbersFromRevision().Should().Have.SameSequenceAs(1, 2);
			QueryForPropertyHasNotChanged(typeof(ParentEntity), id, "Data")
				.ExtractRevisionNumbersFromRevision().Should().Be.Empty();
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Integration.Inheritance.Joined.Mapping.hbm.xml" };
			}
		}
	}
}