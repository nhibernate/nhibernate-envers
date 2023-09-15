using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Components;
using NHibernate.Envers.Tests.Integration.Collection.MapKey;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class HasChangedComponentMapKeyTest : AbstractModifiedFlagsEntityTest
	{
		private int cmke_id;

		private int cte1_id;
		private int cte2_id;

		public HasChangedComponentMapKeyTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var cmke = new ComponentMapKeyEntity();
			var cte1 = new ComponentTestEntity{Comp1 = new Component1 {Str1 = "x1", Str2 = "y2"}, Comp2 = new Component2 {Str5 = "a1", Str6 = "b2"}};
			var cte2 = new ComponentTestEntity{Comp1 = new Component1 {Str1 = "x1", Str2 = "y2"}, Comp2 = new Component2 {Str5 = "a1", Str6 = "b2"}};
			//Revision 1
			using (var tx = Session.BeginTransaction())
			{
				cte1_id = (int) Session.Save(cte1);
				cte2_id = (int) Session.Save(cte2);
				cmke.IdMap[cte1.Comp1] = cte1;
				cmke_id = (int) Session.Save(cmke);
				tx.Commit();
			}
			//revision 2
			using (var tx = Session.BeginTransaction())
			{
				cmke.IdMap[cte2.Comp1] = cte2;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyHasChangedMapEntity()
		{
			QueryForPropertyHasChanged(typeof(ComponentMapKeyEntity), cmke_id, "IdMap")
				.ExtractRevisionNumbersFromRevision().Should().Have.SameSequenceAs(1, 2);
			QueryForPropertyHasNotChanged(typeof(ComponentMapKeyEntity), cmke_id, "IdMap")
				.ExtractRevisionNumbersFromRevision().Should().Be.Empty();
		}

		[Test]
		public void VerifyHasChangedComponentEntity()
		{
			QueryForPropertyHasChanged(typeof(ComponentTestEntity), cte1_id, "Comp1")
				.ExtractRevisionNumbersFromRevision().Should().Have.SameSequenceAs(1);
			QueryForPropertyHasNotChanged(typeof (ComponentTestEntity), cte1_id, "Comp1")
				.ExtractRevisionNumbersFromRevision().Should().Be.Empty();
			QueryForPropertyHasChanged(typeof(ComponentTestEntity), cte2_id, "Comp1")
				.ExtractRevisionNumbersFromRevision().Should().Have.SameSequenceAs(1);
			QueryForPropertyHasNotChanged(typeof(ComponentTestEntity), cte2_id, "Comp1")
				.ExtractRevisionNumbersFromRevision().Should().Be.Empty();
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Integration.Collection.MapKey.Mapping.hbm.xml", "Entities.Components.Mapping.hbm.xml", "Entities.Mapping.hbm.xml" };
			}
		}
	}
}