using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Bugs.NHE12
{
	[TestFixture, Ignore("Not yet solved")]
	public class DynamicComponentTest : TestBase
	{
		private int id;

		protected override void Initialize()
		{
			var c = new DynamicTestEntity();
			c.Properties["Name"] = "Test1";

			using (var tx = Session.BeginTransaction())
			{
				id = (int) Session.Save(c);
				tx.Commit();
			}

			using(var tx = Session.BeginTransaction())
			{
				c.Properties["Name"] = "Test2";
				tx.Commit();
			}

			using(var tx = Session.BeginTransaction())
			{
				c.Properties["Name2"] = "Test";
				c.Properties.Remove("Name");
				tx.Commit();
			}
		}


		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, AuditReader().GetRevisions<DynamicTestEntity>(id));
		}

		[Test]
		public void VerifyHistory()
		{
			var rev1 = AuditReader().Find<DynamicTestEntity>(id, 1);
			var rev2 = AuditReader().Find<DynamicTestEntity>(id, 2);
			var rev3 = AuditReader().Find<DynamicTestEntity>(id, 3);

			rev1.Properties.Count.Should().Be.EqualTo(1);
			rev2.Properties.Count.Should().Be.EqualTo(1);
			rev3.Properties.Count.Should().Be.EqualTo(1);

			rev1.Properties["Name"].Should().Be.EqualTo("Test1");
			rev2.Properties["Name"].Should().Be.EqualTo("Test2");
			rev3.Properties["Name2"].Should().Be.EqualTo("Test");
		}
	}
}