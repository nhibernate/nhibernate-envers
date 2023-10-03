using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.OneToMany;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Cache
{
	public partial class OneToManyCacheTest : TestBase
	{
		private int ed1_id;
		private int ing1_id;
		private int ing2_id;

		public OneToManyCacheTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.OneToMany.Mapping.hbm.xml", "Entities.Mapping.hbm.xml" }; }
		}

		protected override void Initialize()
		{
			var ed1 = new SetRefEdEntity { Id = 1, Data = "data_ed_1" };
			var ed2 = new SetRefEdEntity { Id = 2, Data = "data_ed_2" };

			var ing1 = new SetRefIngEntity { Id = 1, Data = "data_ing_1" };
			var ing2 = new SetRefIngEntity { Id = 2, Data = "data_ing_2" };

			using (var tx = Session.BeginTransaction()) //rev1
			{
				ed1_id = (int)Session.Save(ed1);
				Session.Save(ed2);
				ing1.Reference = ed1;
				ing2.Reference = ed1;
				ing1_id = (int)Session.Save(ing1);
				ing2_id = (int)Session.Save(ing2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction()) //rev2
			{
				ing1.Reference = ed2;
				ing2.Reference = ed2;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyCacheReferenceAfterFind()
		{
			var ed1_rev1 = AuditReader().Find<SetRefEdEntity>(ed1_id, 1);
			var ing1_rev1 = AuditReader().Find<SetRefIngEntity>(ing1_id, 1);
			var ing2_rev1 = AuditReader().Find<SetRefIngEntity>(ing2_id, 1);

			Assert.AreSame(ing1_rev1.Reference, ed1_rev1);
			Assert.AreSame(ing2_rev1.Reference, ed1_rev1);

		}

		[Test]
		public void VerifyCacheReferenceAccessAfterCollectionAccessRev1()
		{
			var ed1_rev1 = AuditReader().Find<SetRefEdEntity>(ed1_id, 1);
			Assert.AreEqual(2, ed1_rev1.Reffering.Count);
			foreach (var setRefIngEntity in ed1_rev1.Reffering)
			{
				Assert.AreSame(setRefIngEntity.Reference, ed1_rev1);
			}
		}

		[Test]
		public void VerifyCacheReferenceAccessAfterCollectionAccessRev2()
		{
			var ed1_rev2 = AuditReader().Find<SetRefEdEntity>(ed1_id, 1);
			Assert.AreEqual(2, ed1_rev2.Reffering.Count);
			foreach (var setRefIngEntity in ed1_rev2.Reffering)
			{
				Assert.AreSame(setRefIngEntity.Reference, ed1_rev2);
			}
		}

		[Test]
		public void VerifyCacheFindAfterCollectionAccessRev1()
		{
			var ed1_rev1 = AuditReader().Find<SetRefEdEntity>(ed1_id, 1);

			// Reading the collection
			Assert.AreEqual(2, ed1_rev1.Reffering.Count);

			var ing1_rev1 = AuditReader().Find<SetRefIngEntity>(ing1_id, 1);
			var ing2_rev1 = AuditReader().Find<SetRefIngEntity>(ing2_id, 1);
			foreach (var setRefIngEntity in ed1_rev1.Reffering)
			{
				if (setRefIngEntity == ing1_rev1 || setRefIngEntity == ing2_rev1)
					continue;
				Assert.Fail();
			}
		}
	}
}