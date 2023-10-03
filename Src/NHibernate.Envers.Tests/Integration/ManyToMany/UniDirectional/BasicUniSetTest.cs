using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.ManyToMany.UniDirectional;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.ManyToMany.UniDirectional
{
	public partial class BasicUniSetTest : TestBase
	{
		private int ed1_id;
		private int ed2_id;
		private int ing1_id;
		private int ing2_id;

		public BasicUniSetTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			ing1_id = 1233;
			ing2_id = 929;

			var ed1 = new StrTestEntity { Str = "data_ed_1" };
			var ed2 = new StrTestEntity { Str = "data_ed_1" };
			var ing1 = new SetUniEntity { Id = ing1_id, Data = "data_ing_1" };
			var ing2 = new SetUniEntity { Id = ing2_id, Data = "data_ing_2" };

			using (var tx = Session.BeginTransaction())
			{
				ed1_id = (int)Session.Save(ed1);
				ed2_id = (int)Session.Save(ed2);
				Session.Save(ing1);
				Session.Save(ing2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ing1.References = new HashSet<StrTestEntity> { ed1 };
				ing2.References = new HashSet<StrTestEntity> { ed1, ed2 };
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ing1.References.Add(ed2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ing1.References.Remove(ed1);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ing1.References = null;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(StrTestEntity), ed1_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(StrTestEntity), ed2_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4, 5 }, AuditReader().GetRevisions(typeof(SetUniEntity), ing1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(SetUniEntity), ing2_id));
		}

		[Test]
		public void VerifyHistoryIng1()
		{
			var ed1 = Session.Get<StrTestEntity>(ed1_id);
			var ed2 = Session.Get<StrTestEntity>(ed2_id);

			var rev1 = AuditReader().Find<SetUniEntity>(ing1_id, 1);
			var rev2 = AuditReader().Find<SetUniEntity>(ing1_id, 2);
			var rev3 = AuditReader().Find<SetUniEntity>(ing1_id, 3);
			var rev4 = AuditReader().Find<SetUniEntity>(ing1_id, 4);
			var rev5 = AuditReader().Find<SetUniEntity>(ing1_id, 5);

			CollectionAssert.IsEmpty(rev1.References);
			CollectionAssert.AreEquivalent(new[] { ed1 }, rev2.References);
			CollectionAssert.AreEquivalent(new[] { ed1, ed2 }, rev3.References);
			CollectionAssert.AreEquivalent(new[] { ed2 }, rev4.References);
			CollectionAssert.IsEmpty(rev5.References);
		}

		[Test]
		public void VerifyHistoryIng2()
		{
			var ed1 = Session.Get<StrTestEntity>(ed1_id);
			var ed2 = Session.Get<StrTestEntity>(ed2_id);

			var rev1 = AuditReader().Find<SetUniEntity>(ing2_id, 1);
			var rev2 = AuditReader().Find<SetUniEntity>(ing2_id, 2);
			var rev3 = AuditReader().Find<SetUniEntity>(ing2_id, 3);
			var rev4 = AuditReader().Find<SetUniEntity>(ing2_id, 4);
			var rev5 = AuditReader().Find<SetUniEntity>(ing2_id, 5);

			CollectionAssert.IsEmpty(rev1.References);
			CollectionAssert.AreEquivalent(new[] { ed1, ed2 }, rev2.References);
			CollectionAssert.AreEquivalent(new[] { ed1, ed2 }, rev3.References);
			CollectionAssert.AreEquivalent(new[] { ed1, ed2 }, rev4.References);
			CollectionAssert.AreEquivalent(new[] { ed1, ed2 }, rev5.References);
		}


		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml", "Entities.ManyToMany.UniDirectional.Mapping.hbm.xml" };
			}
		}
	}
}