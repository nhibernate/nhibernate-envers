using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Components;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Components
{
	public partial class ComponentsTest : TestBase
	{
		private int id1;
		private int id2;
		private int id3;
		private int id4;

		public ComponentsTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Components.Mapping.hbm.xml" }; }
		}

		protected override void Initialize()
		{
			var cte1 = new ComponentTestEntity
			{
				Comp1 = new Component1 { Str1 = "a", Str2 = "b" },
				Comp2 = new Component2 { Str5 = "x", Str6 = "y" }
			};
			var cte2 = new ComponentTestEntity
			{
				Comp1 = new Component1 { Str1 = "a2", Str2 = "b2" },
				Comp2 = new Component2 { Str5 = "x2", Str6 = "y2" }
			};
			var cte3 = new ComponentTestEntity
			{
				Comp1 = new Component1 { Str1 = "a3", Str2 = "b3" },
				Comp2 = new Component2 { Str5 = "x3", Str6 = "y3" }
			};
			var cte4 = new ComponentTestEntity();
			using(var tx = Session.BeginTransaction())
			{
				id1 = (int)Session.Save(cte1);
				id2 = (int)Session.Save(cte2);
				id3 = (int)Session.Save(cte3);
				id4 = (int)Session.Save(cte4);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				cte1.Comp1 = new Component1 {Str1 = "a'", Str2 = "b'"};
				cte2.Comp1.Str1 = "a2'";
				cte3.Comp2.Str6 = "y3'";
				cte4.Comp1 = new Component1 {Str1 = "n"};
				cte4.Comp2 = new Component2 {Str5 = "m"};
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				cte1.Comp2 = new Component2{Str5 = "x'", Str6 = "y'"};
				cte3.Comp1.Str2 = "b3'";
				cte4.Comp1 = null;
				cte4.Comp2 = null;
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				Session.Delete(cte2);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			CollectionAssert.AreEqual(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(ComponentTestEntity), id1));
			CollectionAssert.AreEqual(new[] { 1, 2, 4 }, AuditReader().GetRevisions(typeof(ComponentTestEntity), id2));
			CollectionAssert.AreEqual(new[] { 1, 3 }, AuditReader().GetRevisions(typeof(ComponentTestEntity), id3));
			CollectionAssert.AreEqual(new[] { 1, 2, 3 }, AuditReader().GetRevisions(typeof(ComponentTestEntity), id4));
		}

		[Test]
		public void VerifyHistoryOf1()
		{
			var ver1 = new ComponentTestEntity { Id = id1, Comp1 = new Component1 { Str1 = "a", Str2 = "b" } };
			var ver2 = new ComponentTestEntity { Id = id1, Comp1 = new Component1 { Str1 = "a'", Str2 = "b'" } };

			Assert.AreEqual(ver1, AuditReader().Find<ComponentTestEntity>(id1, 1));
			Assert.AreEqual(ver2, AuditReader().Find<ComponentTestEntity>(id1, 2));
			Assert.AreEqual(ver2, AuditReader().Find<ComponentTestEntity>(id1, 3));
			Assert.AreEqual(ver2, AuditReader().Find<ComponentTestEntity>(id1, 4));
		}

		[Test]
		public void VerifyHistoryOf2()
		{
			var ver1 = new ComponentTestEntity { Id = id2, Comp1 = new Component1 { Str1 = "a2", Str2 = "b2" } };
			var ver2 = new ComponentTestEntity { Id = id2, Comp1 = new Component1 { Str1 = "a2'", Str2 = "b2" } };

			Assert.AreEqual(ver1, AuditReader().Find<ComponentTestEntity>(id2, 1));
			Assert.AreEqual(ver2, AuditReader().Find<ComponentTestEntity>(id2, 2));
			Assert.AreEqual(ver2, AuditReader().Find<ComponentTestEntity>(id2, 3));
			Assert.IsNull(AuditReader().Find<ComponentTestEntity>(id2, 4));
		}

		[Test]
		public void VerifyHistoryOf3()
		{
			var ver1 = new ComponentTestEntity { Id = id3, Comp1 = new Component1 { Str1 = "a3", Str2 = "b3" } };
			var ver2 = new ComponentTestEntity { Id = id3, Comp1 = new Component1 { Str1 = "a3", Str2 = "b3'" } };

			Assert.AreEqual(ver1, AuditReader().Find<ComponentTestEntity>(id3, 1));
			Assert.AreEqual(ver1, AuditReader().Find<ComponentTestEntity>(id3, 2));
			Assert.AreEqual(ver2, AuditReader().Find<ComponentTestEntity>(id3, 3));
			Assert.AreEqual(ver2, AuditReader().Find<ComponentTestEntity>(id3, 4));
		}

		[Test]
		public void VerifyHistoryOf4()
		{
			var ver1 = new ComponentTestEntity { Id = id4 };
			var ver2 = new ComponentTestEntity { Id = id4, Comp1 = new Component1 { Str1 = "n" } };
			var ver3 = new ComponentTestEntity { Id = id4 };

			Assert.AreEqual(ver1, AuditReader().Find<ComponentTestEntity>(id4, 1));
			Assert.AreEqual(ver2, AuditReader().Find<ComponentTestEntity>(id4, 2));
			Assert.AreEqual(ver3, AuditReader().Find<ComponentTestEntity>(id4, 3));
			Assert.AreEqual(ver3, AuditReader().Find<ComponentTestEntity>(id4, 4));
		}
	}
}