using System.Collections.Generic;
using NHibernate.Envers.Tests.Integration.OneToOne.BiDirectional;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class HasChangedBidirectional2Test : AbstractModifiedFlagsEntityTest
	{
		private int ed1Id;
		private int ed2Id;

		public HasChangedBidirectional2Test(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ed1 = new BiRefEdEntity {Id = 1, Data = "data_ed_1"};
			var ed2 = new BiRefEdEntity {Id = 2, Data = "data_ed_2"};
			var ing1 = new BiRefIngEntity {Id = 3, Data = "data_ing_1"};
			var ing2 = new BiRefIngEntity {Id = 4, Data = "data_ing_2"};

			//revision1
			using (var tx = Session.BeginTransaction())
			{
				ed1Id = (int) Session.Save(ed1);
				ed2Id = (int) Session.Save(ed2);
				tx.Commit();
			}

			//revision 2
			using (var tx = Session.BeginTransaction())
			{
				ing1.Reference = ed1;
				Session.Save(ing1);
				Session.Save(ing2);
				tx.Commit();
			}

			//revision 3
			using (var tx = Session.BeginTransaction())
			{
				ing1.Reference = null;
				ing2.Reference = ed1;
				tx.Commit();
			}

			//revision 4
			using (var tx = Session.BeginTransaction())
			{
				ing1.Reference = ed2;
				ing2.Reference = null;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyHasChanged()
		{
			QueryForPropertyHasChanged(typeof (BiRefEdEntity), ed1Id, "Referencing")
				.ExtractRevisionNumbersFromRevision().Should().Have.SameSequenceAs(2, 3, 4);

			QueryForPropertyHasChanged(typeof (BiRefEdEntity), ed2Id, "Referencing")
				.ExtractRevisionNumbersFromRevision().Should().Have.SameSequenceAs(4);
		}


		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Integration.OneToOne.BiDirectional.Mapping.hbm.xml" };
			}
		}
	}
}