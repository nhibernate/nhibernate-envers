using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.OneToMany.Detached;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class HasChangedDoubleJoinColumnBidirectionalListTest : AbstractModifiedFlagsEntityTest
	{
		private int ed1_1_id;
		private int ed2_1_id;
		private int ed1_2_id;
		private int ed2_2_id;
		private int ing1_id;
		private int ing2_id;

		public HasChangedDoubleJoinColumnBidirectionalListTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ed1_1 = new DoubleListJoinColumnBidirectionalRefEdEntity1 {Data = "ed1_1"};
			var ed1_2 = new DoubleListJoinColumnBidirectionalRefEdEntity1 {Data = "ed1_2"};
			var ed2_1 = new DoubleListJoinColumnBidirectionalRefEdEntity2 {Data = "ed2_1"};
			var ed2_2 = new DoubleListJoinColumnBidirectionalRefEdEntity2 {Data = "ed2_2"};
			var ing1 = new DoubleListJoinColumnBidirectionalRefIngEntity {Data = "coll1"};
			var ing2 = new DoubleListJoinColumnBidirectionalRefIngEntity {Data = "coll2"};

			// Revision 1 (ing1: ed1_1, ed2_1, ing2: ed1_2, ed2_2)
			using (var tx = Session.BeginTransaction())
			{
				ing1.References1.Add(ed1_1);
				ing1.References2.Add(ed2_1);
				ing2.References1.Add(ed1_2);
				ing2.References2.Add(ed2_2);
				ed1_1_id = (int) Session.Save(ed1_1);
				ed1_2_id = (int) Session.Save(ed1_2);
				ed2_1_id = (int) Session.Save(ed2_1);
				ed2_2_id = (int) Session.Save(ed2_2);
				ing1_id = (int) Session.Save(ing1);
				ing2_id = (int) Session.Save(ing2);
				tx.Commit();
			}

			// Revision 2 (ing1: ed1_1, ed1_2, ed2_1, ed2_2)
			using (var tx = Session.BeginTransaction())
			{
				ing2.References1.Clear();
				ing2.References2.Clear();
				ing1.References1.Add(ed1_2);
				ing1.References2.Add(ed2_2);
				tx.Commit();
			}

			// Revision 3 (ing1: ed1_1, ed1_2, ed2_1, ed2_2)
			using (var tx = Session.BeginTransaction())
			{
				ed1_1.Data = "ed1_1 bis";
				ed2_2.Data = "ed2_2 bis";
				tx.Commit();
			}

			// Revision 4 (ing1: ed2_2, ing2: ed2_1, ed1_1, ed1_2)
			using (var tx = Session.BeginTransaction())
			{
				ing1.References1.Clear();
				ing2.References1.Add(ed1_1);
				ing2.References1.Add(ed1_2);
				ing1.References2.Remove(ed2_1);
				ing2.References2.Add(ed2_1);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyOwnerHasChanged()
		{
			QueryForPropertyHasChanged(typeof (DoubleListJoinColumnBidirectionalRefEdEntity1), ed1_1_id, "Owner")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 4);
			QueryForPropertyHasNotChanged(typeof(DoubleListJoinColumnBidirectionalRefEdEntity1), ed1_1_id, "Owner")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(3);
			QueryForPropertyHasChanged(typeof(DoubleListJoinColumnBidirectionalRefEdEntity1), ed1_2_id, "Owner")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 2, 4);
			QueryForPropertyHasNotChanged(typeof(DoubleListJoinColumnBidirectionalRefEdEntity1), ed1_2_id, "Owner")
				.ExtractRevisionNumbersFromRevision()
				.Should().Be.Empty();
		}

		[Test]
		public void VerifyOwnerSecEntityHasChanged()
		{
			QueryForPropertyHasChanged(typeof(DoubleListJoinColumnBidirectionalRefEdEntity2), ed2_1_id, "Owner")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 4);
			QueryForPropertyHasNotChanged(typeof(DoubleListJoinColumnBidirectionalRefEdEntity2), ed2_1_id, "Owner")
				.ExtractRevisionNumbersFromRevision()
				.Should().Be.Empty();
			QueryForPropertyHasChanged(typeof(DoubleListJoinColumnBidirectionalRefEdEntity2), ed2_2_id, "Owner")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 2);
			QueryForPropertyHasNotChanged(typeof(DoubleListJoinColumnBidirectionalRefEdEntity2), ed2_2_id, "Owner")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(3);
		}

		[Test]
		public void VerifyReferences1HasChanged()
		{
			QueryForPropertyHasChanged(typeof(DoubleListJoinColumnBidirectionalRefIngEntity), ing1_id, "References1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 2, 4);
			QueryForPropertyHasChanged(typeof(DoubleListJoinColumnBidirectionalRefIngEntity), ing2_id, "References1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 2, 4);
			QueryForPropertyHasNotChanged(typeof(DoubleListJoinColumnBidirectionalRefIngEntity), ing1_id, "References1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Be.Empty();
			QueryForPropertyHasNotChanged(typeof(DoubleListJoinColumnBidirectionalRefIngEntity), ing2_id, "References1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Be.Empty();
		}

		[Test]
		public void VerifyReferences2HasChanged()
		{
			QueryForPropertyHasChanged(typeof(DoubleListJoinColumnBidirectionalRefIngEntity), ing1_id, "References2")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 2, 4);
			QueryForPropertyHasChanged(typeof(DoubleListJoinColumnBidirectionalRefIngEntity), ing2_id, "References2")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 2, 4);
			QueryForPropertyHasNotChanged(typeof(DoubleListJoinColumnBidirectionalRefIngEntity), ing1_id, "References2")
				.ExtractRevisionNumbersFromRevision()
				.Should().Be.Empty();
			QueryForPropertyHasNotChanged(typeof(DoubleListJoinColumnBidirectionalRefIngEntity), ing2_id, "References2")
				.ExtractRevisionNumbersFromRevision()
				.Should().Be.Empty();
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.OneToMany.Detached.Mapping.hbm.xml", "Entities.Mapping.hbm.xml" };
			}
		}
	}
}