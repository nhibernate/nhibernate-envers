using System.Collections.Generic;
using NHibernate.Envers.Tests.Integration.OneToOne.BiDirectional;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Cache
{
	public partial class OneToOneCacheTest : TestBase
	{
		private int ed1_id;
		private int ing1_id;
		private int ed2_id;

		public OneToOneCacheTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Integration.OneToOne.BiDirectional.Mapping.hbm.xml" }; }
		}

		protected override void Initialize()
		{
			var ed1 = new BiRefEdEntity { Id = 1, Data = "data_ed_1" };
			var ed2 = new BiRefEdEntity { Id = 2, Data = "data_ed_2" };
			var ing1 = new BiRefIngEntity { Id = 3, Data = "data_ing_1" };

			using(var tx = Session.BeginTransaction())
			{
				ing1.Reference = ed1;
				ed1_id = (int)Session.Save(ed1);
				ed2_id = (int)Session.Save(ed2);
				ing1_id = (int)Session.Save(ing1);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				ing1.Reference = ed2;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyCacheReferenceAccessAfterFindRev1() 
		{
			var ed1_rev1 = AuditReader().Find<BiRefEdEntity>(ed1_id, 1);
			var ing1_rev1 = AuditReader().Find<BiRefIngEntity>(ing1_id, 1);

			Assert.AreSame(ing1_rev1.Reference, ed1_rev1);
		}

		[Test]
		public void VerifyCacheReferenceAccessAfterFindRev2()
		{
			var ed2_rev2 = AuditReader().Find<BiRefEdEntity>(ed2_id, 2);
			var ing1_rev2 = AuditReader().Find<BiRefIngEntity>(ing1_id, 2);

			Assert.AreSame(ing1_rev2.Reference, ed2_rev2);
		}
	}
}
