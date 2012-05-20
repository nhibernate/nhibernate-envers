using NHibernate.Envers.Tests.Integration.ModifiedFlags;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.DynamicComponent
{
	[Ignore("Currently not supported")]
	public class HasChangedDynamicComponentTest : AbstractModifiedFlagsEntityTest
	{
		private int id;

		public HasChangedDynamicComponentTest(string strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var c = new DynamicTestEntity();
			c.Properties["Name"] = "1";

			using (var tx = Session.BeginTransaction())
			{
				id = (int)Session.Save(c);
				tx.Commit();
			}

			using (var tx = Session.BeginTransaction())
			{
				c.Properties["Name2"] = "2";
				tx.Commit();
			}	
		}

		[Test]
		public void VerifyHasChanged()
		{
			QueryForPropertyHasChanged(typeof (DynamicTestEntity), id, "Properties")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1);
		}

		[Test]
		public void VerifyHasNotChanged()
		{
			QueryForPropertyHasNotChanged(typeof(DynamicTestEntity), id, "Properties")
				.ExtractRevisionNumbersFromRevision()
				.Should().Be.Empty();
		}

		protected override System.Collections.Generic.IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "NetSpecific.Integration.DynamicComponent.SingleProperty.hbm.xml" };
			}
		}
	}
}