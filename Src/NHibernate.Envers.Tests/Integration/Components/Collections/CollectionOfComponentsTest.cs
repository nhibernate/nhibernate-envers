using NHibernate.Envers.Tests.Entities.Components;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Components.Collections
{
	[TestFixture]
	[Ignore("Collection of components not working right now")]
	public class CollectionOfComponentsTest : TestBase
	{
		private int id1;

		protected override void Initialize()
		{
			var cte1 = new ComponentSetTestEntity();
			using(var tx = Session.BeginTransaction())
			{
				id1 = (int)Session.Save(cte1);
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
			CollectionAssert.AreEqual(new[] { 1, 2 }, AuditReader.GetRevisions(typeof(ComponentSetTestEntity), id1));
		}

		[Test]
		public void VerifyHistoryOf1()
		{
			CollectionAssert.IsEmpty(AuditReader.Find<ComponentSetTestEntity>(id1, 1).Comps);

			var comps1 = AuditReader.Find<ComponentSetTestEntity>(id1, 2).Comps;
			Assert.AreEqual(1, comps1.Count);
			CollectionAssert.Contains(comps1, new Component1 { Str1 = "a", Str2 = "b" });
		}
	}
}