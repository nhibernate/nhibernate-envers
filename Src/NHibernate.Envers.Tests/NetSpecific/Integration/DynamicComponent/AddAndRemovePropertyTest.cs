using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.DynamicComponent
{
	public partial class AddAndRemovePropertyTest : TestBase
	{
		private int id;

		public AddAndRemovePropertyTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

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

			//only "Name" is mapped
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
			AuditReader().GetRevisions(typeof (DynamicTestEntity), id)
				.Should().Have.SameSequenceAs(1, 2, 3);
		}

		[Test]
		public void VerifyHistory()
		{
			var rev1 = AuditReader().Find<DynamicTestEntity>(id, 1);
			var rev2 = AuditReader().Find<DynamicTestEntity>(id, 2);
			var rev3 = AuditReader().Find<DynamicTestEntity>(id, 3);

			rev1.Properties.Count.Should().Be.EqualTo(1);
			rev2.Properties.Count.Should().Be.EqualTo(1);
			rev3.Properties.Should().Be.Null();

			rev1.Properties["Name"].Should().Be.EqualTo("1");
			rev2.Properties["Name"].Should().Be.EqualTo("2");
		}

		protected override System.Collections.Generic.IEnumerable<string> Mappings
		{
			get
			{
				return new[]{"NetSpecific.Integration.DynamicComponent.SingleProperty.hbm.xml"};
			}
		}
	}
}