using System.Collections;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.DynamicComponent
{
	public partial class NonExistingKeyTest : TestBase
	{
		private int id;

		public NonExistingKeyTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var entity = new DynamicTestEntity();
			//Revision 1 - Properties is null
			using (var tx = Session.BeginTransaction())
			{
				entity.Properties = null;
				id = (int) Session.Save(entity);
				tx.Commit();
				entity.Properties = new Hashtable();
			}
			//Revision 2 = all properties set
			using (var tx = Session.BeginTransaction())
			{
				entity.Properties.Add("Prop1", 1);
				entity.Properties.Add("Prop2", 2);
				entity.Properties.Add("Prop3", 3);
				tx.Commit();
			}
			//Revision 3 - removed one property
			using (var tx = Session.BeginTransaction())
			{
				entity.Properties.Remove("Prop1");
				tx.Commit();
			}
			//Revision 4 - properties is empty
			using (var tx = Session.BeginTransaction())
			{
				entity.Properties.Clear();
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevision1()
		{
			var rev = AuditReader().Find<DynamicTestEntity>(id, 1);
			rev.Properties.Should().Be.Null();
		}

		[Test]
		public void VerifyRevision2()
		{
			var rev = AuditReader().Find<DynamicTestEntity>(id, 2);
			rev.Properties["Prop1"].Should().Be.EqualTo(1);
			rev.Properties["Prop2"].Should().Be.EqualTo(2);
			rev.Properties["Prop3"].Should().Be.EqualTo(3);
		}

		[Test]
		public void VerifyRevision3()
		{
			var rev = AuditReader().Find<DynamicTestEntity>(id, 3);
			rev.Properties.Contains("Prop1").Should().Be.False();
			rev.Properties["Prop2"].Should().Be.EqualTo(2);
			rev.Properties["Prop3"].Should().Be.EqualTo(3);
		}

		[Test]
		public void VerifyRevision4()
		{
			var rev = AuditReader().Find<DynamicTestEntity>(id, 4);
			rev.Properties.Should().Be.Null();
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