using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.OneToMany;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToMany
{
	public partial class InverseSideChangesTest : TestBase
	{
		private const int ed1_id =123123;
		private const int ing1_id = 33;

		public InverseSideChangesTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ed1 = new SetRefEdEntity {Id = ed1_id, Data = "data_ed_1"};
			var ing1 = new SetRefIngEntity { Id = ing1_id, Data = "data_ing_1" };
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ed1);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ing1);
				ed1.Reffering = new HashSet<SetRefIngEntity> {ing1};
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(SetRefEdEntity), ed1_id));
			CollectionAssert.AreEquivalent(new[] { 2 }, AuditReader().GetRevisions(typeof(SetRefIngEntity), ing1_id));
		}

		[Test]
		public void VerifyHistoryOfEd1()
		{
			var rev1 = AuditReader().Find<SetRefEdEntity>(ed1_id, 1);

			CollectionAssert.IsEmpty(rev1.Reffering);
		}

		[Test]
		public void VerifyHistoryOfIng1()
		{
			var rev2 = AuditReader().Find<SetRefIngEntity>(ing1_id, 2);

			Assert.IsNull(rev2.Reference);
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