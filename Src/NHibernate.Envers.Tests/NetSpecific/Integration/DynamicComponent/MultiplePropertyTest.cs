using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.DynamicComponent
{
	public partial class MultiplePropertyTest : TestBase
	{
		private int id;

		public MultiplePropertyTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ent = new DynamicTestEntity();
			ent.Properties["Prop1"] = 1;
			ent.Properties["Prop2"] = 2;
			ent.Properties["Prop3"] = 3;
			using (var tx = Session.BeginTransaction())
			{
				id = (int) Session.Save(ent);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ent.Properties["Prop3"] = 33;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			AuditReader().GetRevisions(typeof(DynamicTestEntity), id)
				.Should().Have.SameSequenceAs(1, 2);
		}

		[Test]
		public void VerifyRevision1()
		{
			var rev = AuditReader().Find<DynamicTestEntity>(id, 1);

			rev.Properties["Prop1"].Should().Be.EqualTo(1);
			rev.Properties["Prop2"].Should().Be.EqualTo(2);
			rev.Properties["Prop3"].Should().Be.EqualTo(3);
		}

		[Test]
		public void VerifyRevision2()
		{
			var rev = AuditReader().Find<DynamicTestEntity>(id, 2);

			rev.Properties["Prop1"].Should().Be.EqualTo(1);
			rev.Properties["Prop2"].Should().Be.EqualTo(2);
			rev.Properties["Prop3"].Should().Be.EqualTo(33);
		}

		protected override System.Collections.Generic.IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "NetSpecific.Integration.DynamicComponent.MultipleProperty.hbm.xml" };
			}
		}
	}
}