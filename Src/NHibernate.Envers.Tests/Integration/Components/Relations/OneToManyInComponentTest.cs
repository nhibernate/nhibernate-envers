using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.Components.Relations;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Components.Relations
{
	public partial class OneToManyInComponentTest : TestBase
	{
		private int otmcte_id1;
		private int ste_id1;
		private int ste_id2;

		public OneToManyInComponentTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Mapping.hbm.xml", "Entities.Components.Relations.Mapping.hbm.xml" }; }
		}

		protected override void Initialize()
		{
			var ste1 = new StrTestEntity { Str = "str1" };
			var ste2 = new StrTestEntity { Str = "str2" };
			var otmcte1 = new OneToManyComponentTestEntity {Comp1 = new OneToManyComponent {Data = "data1"}};
			using(var tx = Session.BeginTransaction())
			{
				ste_id1 = (int) Session.Save(ste1);
				ste_id2 = (int) Session.Save(ste2);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				otmcte1.Comp1.Entities.Add(ste1);
				otmcte_id1 = (int)Session.Save(otmcte1);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				otmcte1.Comp1.Entities.Add(ste2);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			CollectionAssert.AreEqual(new[] { 2, 3 }, AuditReader().GetRevisions(typeof(OneToManyComponentTestEntity), otmcte_id1));
		}

		[Test]
		public void VerifyHistoryOf1()
		{
			var ste1 = Session.Get<StrTestEntity>(ste_id1);
			var ste2 = Session.Get<StrTestEntity>(ste_id2);

			var ver2 = new OneToManyComponentTestEntity {Id = otmcte_id1, Comp1 = new OneToManyComponent {Data = "data1"}};
			ver2.Comp1.Entities.Add(ste1);
			var ver3 = new OneToManyComponentTestEntity {Id = otmcte_id1, Comp1 = new OneToManyComponent {Data = "data1"}};
			ver3.Comp1.Entities.Add(ste1);
			ver3.Comp1.Entities.Add(ste2);

			Assert.IsNull(AuditReader().Find<OneToManyComponentTestEntity>(otmcte_id1, 1));
			Assert.AreEqual(ver2, AuditReader().Find<OneToManyComponentTestEntity>(otmcte_id1, 2));
			Assert.AreEqual(ver3, AuditReader().Find<OneToManyComponentTestEntity>(otmcte_id1, 3));
		}
	}
}