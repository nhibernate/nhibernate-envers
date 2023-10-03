using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Components;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Collection.MapKey
{
	public partial class ComponentMapKeyTest : TestBase
	{
		private int cmke_id;
		private int cte1_id;
		private int cte2_id;

		public ComponentMapKeyTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Integration.Collection.MapKey.Mapping.hbm.xml", 
								"Entities.Mapping.hbm.xml", 
								"Entities.Components.Mapping.hbm.xml" };
			}
		}

		protected override void Initialize()
		{
			var imke = new ComponentMapKeyEntity();
			var cte1 = new ComponentTestEntity
			           	{
			           		Comp1 = new Component1 {Str1 = "x1", Str2 = "y2"},
			           		Comp2 = new Component2 {Str5 = "a1", Str6 = "b2"}
			           	};
			var cte2 = new ComponentTestEntity
			           	{
							//changed from Envers test here. Doubt that org test is doing the right thing...
			           		Comp1 = new Component1 {Str1 = "x11", Str2 = "y2"},
			           		Comp2 = new Component2 {Str5 = "a1", Str6 = "b2"}
			           	};
			using(var tx = Session.BeginTransaction())
			{
				cte1_id = (int)Session.Save(cte1);
				cte2_id = (int)Session.Save(cte2);
				imke.IdMap[cte1.Comp1] = cte1;
				cmke_id = (int)Session.Save(imke);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				imke.IdMap[cte2.Comp1] = cte2;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(ComponentMapKeyEntity), cmke_id));
		}

		[Test]
		public void VerifyHistoryOfImke()
		{
			var cte1 = Session.Get<ComponentTestEntity>(cte1_id);
			var cte2 = Session.Get<ComponentTestEntity>(cte2_id);

			// These fields are unversioned.
			cte1.Comp2 = null;
			cte2.Comp2 = null;

			var rev1 = AuditReader().Find<ComponentMapKeyEntity>(cmke_id, 1);
			var rev2 = AuditReader().Find<ComponentMapKeyEntity>(cmke_id, 2);

			Assert.AreEqual(new Dictionary<Component1, ComponentTestEntity> { { cte1.Comp1, cte1 } }, rev1.IdMap);
			Assert.AreEqual(new Dictionary<Component1, ComponentTestEntity> { { cte1.Comp1, cte1 }, { cte2.Comp1, cte2 } }, rev2.IdMap);
		}
	}
}