using Iesi.Collections.Generic;
using NHibernate.Envers.Tests.Entities.OneToMany;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToMany
{
	[TestFixture]
	public class InverseSideChangesTest : TestBase
	{
		private const int ed1_id =123123;
		private const int ing1_id = 33;

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
				ed1.Reffering = new HashedSet<SetRefIngEntity>();
				ed1.Reffering.Add(ing1);
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

		protected override System.Collections.Generic.IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.OneToMany.Mapping.hbm.xml" };
			}
		}
	}
}