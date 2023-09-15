using System.Collections.Generic;
using NHibernate.Envers.Tests.Integration.Inheritance.Entities;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class HasChangedChildReferencingTest : AbstractModifiedFlagsEntityTest
	{
		private int id1;
		private int id2;

		public HasChangedChildReferencingTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			id1 = 1;
			id2 = 10;
			const int cId = 100;

			//rev1
			var re1 = new ReferencedToChildEntity {Id = id1};
			var re2 = new ReferencedToChildEntity {Id = id2};
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(re1);
				Session.Save(re2);
				tx.Commit();
			}
			//rev2
			var cie = new ChildIngEntity { Id = cId, Data = "y", Number = 11, Referenced = re1 };
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(cie);
				tx.Commit();
			}
			//rev3
			using (var tx = Session.BeginTransaction())
			{
				cie.Referenced = re2;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyReferencedEntityHasChanged()
		{
			QueryForPropertyHasChanged(typeof (ReferencedToChildEntity), id1, "Referencing")
				.ExtractRevisionNumbersFromRevision().Should().Have.SameSequenceAs(2, 3);
			QueryForPropertyHasNotChanged(typeof(ReferencedToChildEntity), id1, "Referencing")
				.ExtractRevisionNumbersFromRevision().Should().Have.SameSequenceAs(1);
			QueryForPropertyHasChanged(typeof(ReferencedToChildEntity), id2, "Referencing")
				.ExtractRevisionNumbersFromRevision().Should().Have.SameSequenceAs(3);
		}



		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Integration.Inheritance.Joined.ChildRelation.Mapping.hbm.xml" };
			}
		}
	}
}