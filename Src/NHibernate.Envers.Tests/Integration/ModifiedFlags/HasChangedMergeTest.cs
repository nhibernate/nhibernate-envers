using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.OneToMany;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class HasChangedMergeTest : AbstractModifiedFlagsEntityTest 
	{
		private const int parentId = 11;
		private const int childId = 12;

		public HasChangedMergeTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			// Revision 1 - data preparation
			// Empty collection is not the same as null reference.
			var parent = new CollectionRefEdEntity { Id = parentId, Data = "initial data", Reffering = new List<CollectionRefIngEntity>() };
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(parent);
				tx.Commit();
			}

			//Revision 2 - inserting new child entity and updating parent
			using (var tx = Session.BeginTransaction())
			{
				var child = new CollectionRefIngEntity {Id = childId, Data = "initial data", Reference = parent};
				Session.Save(child);
				parent.Data = "updated data";
				Session.Merge(parent);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyOneToManyInsertChildUpdateParent()
		{
			var list = QueryForPropertyHasChanged(typeof (CollectionRefEdEntity), parentId, "Data");
			list.Count.Should().Be.EqualTo(2);
			list.ExtractRevisionNumbersFromRevision().Should().Have.SameSequenceAs(1, 2);

			list = QueryForPropertyHasChanged(typeof(CollectionRefEdEntity), parentId, "Reffering");
			list.Count.Should().Be.EqualTo(2);
			list.ExtractRevisionNumbersFromRevision().Should().Have.SameSequenceAs(1, 2);

			list = QueryForPropertyHasChanged(typeof(CollectionRefIngEntity), childId, "Reference");
			list.Count.Should().Be.EqualTo(1);
			list.ExtractRevisionNumbersFromRevision().Should().Have.SameSequenceAs(2);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.OneToMany.Mapping.hbm.xml", "Entities.Mapping.hbm.xml" };
			}
		}
	}
}