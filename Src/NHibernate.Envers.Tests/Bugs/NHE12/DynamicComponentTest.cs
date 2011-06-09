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
			c.Properties["Name"] = "1";

			using (var tx = Session.BeginTransaction())
			{
				id = (int) Session.Save(c);
				tx.Commit();
			}

			using(var tx = Session.BeginTransaction())
			{
				c.Properties["Name"] = "2";
				tx.Commit();
			}

			using(var tx = Session.BeginTransaction())
			{
				c.Properties["Name2"] = "3";
				c.Properties["Name3"] = "4";
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
			rev3.Properties.Count.Should().Be.EqualTo(2);

			rev1.Properties["Name"].Should().Be.EqualTo("1");
			rev2.Properties["Name"].Should().Be.EqualTo("2");
			rev3.Properties["Name2"].Should().Be.EqualTo("3");
			rev3.Properties["Name3"].Should().Be.EqualTo("4");
		}
	}
}