using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.CustomType;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class HasChangedCompositeCustomTest : AbstractModifiedFlagsEntityTest
	{
		private int id;

		public HasChangedCompositeCustomTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ccte = new CompositeCustomTypeEntity();

			//Revision 1 (persisting 1 entity)
			using (var tx = Session.BeginTransaction())
			{
				ccte.Component = new Component{Prop1 = "a", Prop2 = 1};
				id = (int) Session.Save(ccte);
				tx.Commit();
			}

			//Revsion 2 (changing the component)
			using (var tx = Session.BeginTransaction())
			{
				ccte.Component.Prop1 = "b";
				tx.Commit();
			}

			//Revision 3 (replacing the component)
			using (var tx = Session.BeginTransaction())
			{
				ccte.Component = new Component {Prop1 = "c", Prop2 = 3};
				tx.Commit();
			}
		}

		[Test]
		public void VerifyHasChanged()
		{
			QueryForPropertyHasChanged(typeof (CompositeCustomTypeEntity), id, "Component")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 2, 3);

			QueryForPropertyHasNotChanged(typeof(CompositeCustomTypeEntity), id, "Component")
				.ExtractRevisionNumbersFromRevision()
				.Should().Be.Empty();
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.CustomType.Mapping.hbm.xml" };
			}
		}
	}
}