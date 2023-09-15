using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.Components.Relations;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class HasChangedOneToManyInComponentTest : AbstractModifiedFlagsEntityTest
	{
		private int id;

		public HasChangedOneToManyInComponentTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ste1 = new StrTestEntity {Str = "str1"};
			var ste2 = new StrTestEntity {Str = "str2"};

			//revision 1
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ste1);
				Session.Save(ste2);
				tx.Commit();
			}
			//revision 2
			var otmcte1 = new OneToManyComponentTestEntity { Comp1 = new OneToManyComponent { Data = "data 1" } };
			using (var tx = Session.BeginTransaction())
			{
				otmcte1.Comp1.Entities.Add(ste1);
				id = (int) Session.Save(otmcte1);
				tx.Commit();
			}
			//revision3
			using (var tx = Session.BeginTransaction())
			{
				otmcte1.Comp1.Entities.Add(ste2);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyHasChanged()
		{
			QueryForPropertyHasChanged(typeof (OneToManyComponentTestEntity), id, "Comp1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(2, 3);

			QueryForPropertyHasNotChanged(typeof (OneToManyComponentTestEntity), id, "Comp1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Be.Empty();
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[]
				       	{
				       		"Entities.Components.Relations.Mapping.hbm.xml",
				       		"Entities.Mapping.hbm.xml"
				       	};
			}
		}
	}
}