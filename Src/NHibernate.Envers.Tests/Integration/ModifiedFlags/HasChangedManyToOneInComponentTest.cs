using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.Components.Relations;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class HasChangedManyToOneInComponentTest : AbstractModifiedFlagsEntityTest
	{
		private int id;

		public HasChangedManyToOneInComponentTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ste1 = new StrTestEntity {Str = "str1"};
			var ste2 = new StrTestEntity {Str = "str2"};

			// Revision 1
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ste1);
				Session.Save(ste2);
				tx.Commit();
			}

			var mtocte1 = new ManyToOneComponentTestEntity {Comp1 = new ManyToOneComponent {Data = "data1", Entity = ste1}};
			//revision 2
			using (var tx = Session.BeginTransaction())
			{
				id = (int) Session.Save(mtocte1);
				tx.Commit();
			}

			// Revision 3
			using (var tx = Session.BeginTransaction())
			{
				mtocte1.Comp1.Entity = ste2;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyHasChanged()
		{
			QueryForPropertyHasChanged(typeof (ManyToOneComponentTestEntity), id, "Comp1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(2, 3);
			QueryForPropertyHasNotChanged(typeof(ManyToOneComponentTestEntity), id, "Comp1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Be.Empty();
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml", 
									"Entities.Components.Relations.Mapping.hbm.xml" };
			}
		}
	}
}