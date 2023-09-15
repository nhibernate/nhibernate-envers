using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.OneToMany;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Merge
{
	public partial class AddEditCollectionItemTest : TestBase
	{
		private const int entityId = 342;
		private const int childId = 1000;

		public AddEditCollectionItemTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var entity = new SetRefEdEntity { Id = entityId, Data = "data" };
			var child = new SetRefIngEntity {Id = childId, Data = "data", Reference = entity};

			//revision 1
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(entity);
				tx.Commit();
			}

			//revision 2
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(child);
				child.Data = "changed data";
				tx.Commit();
			}
		}

		[Test]
		public void RevisionForOwned()
		{
			AuditReader().GetRevisions(typeof(SetRefEdEntity), entityId)
				.Should().Have.SameSequenceAs(1,2);
		}


		[Test]
		public void RevisionForChild()
		{
			AuditReader().GetRevisions(typeof(SetRefIngEntity), childId)
				.Should().Have.SameSequenceAs(2);
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.OneToMany.Mapping.hbm.xml", "Entities.Mapping.hbm.xml" }; }
		}
	}
}