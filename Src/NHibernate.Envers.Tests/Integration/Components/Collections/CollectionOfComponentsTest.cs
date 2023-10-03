using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Components;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Components.Collections
{
	public partial class CollectionOfComponentsTest : TestBase
	{
		private int id1;
		private int id2;

		public CollectionOfComponentsTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var cte1 = new ComponentSetTestEntity();
			var cte2 = new ComponentSetTestEntity();
			cte2.Comps.Add(new Component1 {Str1 = "string1", Str2 = null});
			using(var tx = Session.BeginTransaction())
			{
				id1 = (int)Session.Save(cte1);
				id2 = (int) Session.Save(cte2);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				cte1.Comps.Add(new Component1 {Str1 = "a", Str2 = "b"});
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			CollectionAssert.AreEqual(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(ComponentSetTestEntity), id1));
		}

		[Test]
		public void VerifyHistoryOf1()
		{
			CollectionAssert.IsEmpty(AuditReader().Find<ComponentSetTestEntity>(id1, 1).Comps);

			var comps1 = AuditReader().Find<ComponentSetTestEntity>(id1, 2).Comps;
			Assert.AreEqual(1, comps1.Count);
			CollectionAssert.Contains(comps1, new Component1 { Str1 = "a", Str2 = "b" });
		}

		[Test]
		public void VerifyCollectionOfEmbeddableWithNullValue()
		{
			var componentV1 = new Component1 {Str1 = "string1", Str2 = null};
			var entityV1 = AuditReader().Find<ComponentSetTestEntity>(id2, 1);

			entityV1.Comps.Should().Have.SameSequenceAs(componentV1);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Components.Mapping.hbm.xml" };
			}
		}
	}
}