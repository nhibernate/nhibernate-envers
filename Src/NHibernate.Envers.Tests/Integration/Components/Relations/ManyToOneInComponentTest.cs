using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.Components.Relations;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Components.Relations
{
	public partial class ManyToOneInComponentTest : TestBase
	{
		private int mtocte_id1;
		private int ste_id1;
		private int ste_id2;

		public ManyToOneInComponentTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Components.Relations.Mapping.hbm.xml", "Entities.Mapping.hbm.xml" }; }
		}

		protected override void Initialize()
		{
			var ste1 = new StrTestEntity { Str = "Str1" };
			var ste2 = new StrTestEntity { Str = "Str2" };
			var mtocte1 = new ManyToOneComponentTestEntity { Comp1 = new ManyToOneComponent { Data = "data1", Entity = ste1 } };

			using (var tx = Session.BeginTransaction())
			{
				ste_id1 = (int)Session.Save(ste1);
				ste_id2 = (int)Session.Save(ste2);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				mtocte_id1 = (int)Session.Save(mtocte1);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				mtocte1.Comp1.Entity = ste2;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			CollectionAssert.AreEqual(new[] { 2, 3 }, AuditReader().GetRevisions(typeof(ManyToOneComponentTestEntity), mtocte_id1));
		}

		[Test]
		public void VerifyHistory()
		{
			var ste1 = Session.Get<StrTestEntity>(ste_id1);
			var ste2 = Session.Get<StrTestEntity>(ste_id2);

			var ver2 = new ManyToOneComponentTestEntity { Id = mtocte_id1, Comp1 = new ManyToOneComponent{Entity = ste1, Data = "data1" } };
			var ver3 = new ManyToOneComponentTestEntity { Id = mtocte_id1, Comp1 = new ManyToOneComponent{Entity = ste2, Data = "data1" } };

			Assert.IsNull(AuditReader().Find<ManyToOneComponentTestEntity>(mtocte_id1, 1));
			Assert.AreEqual(ver2, AuditReader().Find<ManyToOneComponentTestEntity>(mtocte_id1, 2));
			Assert.AreEqual(ver3, AuditReader().Find<ManyToOneComponentTestEntity>(mtocte_id1, 3));
		}
	}
}