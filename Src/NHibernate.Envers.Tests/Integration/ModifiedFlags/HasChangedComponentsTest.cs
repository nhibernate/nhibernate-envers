using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Components;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class HasChangedComponentsTest : AbstractModifiedFlagsEntityTest
	{
		private int id1;
		private int id2;
		private int id3;
		private int id4;

		public HasChangedComponentsTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var cte1 = new ComponentTestEntity {Comp1 = new Component1 {Str1 = "a", Str2 = "b"}, Comp2 = new Component2 {Str5 = "x", Str6 = "y"}};
			var cte2 = new ComponentTestEntity {Comp1 = new Component1 {Str1 = "a2", Str2 = "b2"}, Comp2 = new Component2 {Str5 = "x2", Str6 = "y2"}};
			var cte3 = new ComponentTestEntity {Comp1 = new Component1 {Str1 = "a3", Str2 = "b3"}, Comp2 = new Component2 {Str5 = "x3", Str6 = "y3"}};
			var cte4 = new ComponentTestEntity();

			//Rev 1
			using (var tx = Session.BeginTransaction())
			{
				id1 = (int) Session.Save(cte1);
				id2 = (int) Session.Save(cte2);
				id3 = (int) Session.Save(cte3);
				id4 = (int) Session.Save(cte4);
				tx.Commit();
			}

			//rev2
			using (var tx = Session.BeginTransaction())
			{
				cte1.Comp1 = new Component1 {Str1 = "a'", Str2 = "b'"};
				cte2.Comp1.Str1 = "a2'";
				cte3.Comp2.Str6 = "y3'";
				cte4.Comp1 = new Component1 {Str1 = "n"};
				cte4.Comp2 = new Component2 {Str5 = "m"};
				tx.Commit();
			}

			//rev 3
			using (var tx = Session.BeginTransaction())
			{
				cte1.Comp2 = new Component2 {Str5 = "x'", Str6 = "y'"};
				cte3.Comp1.Str2 = "b3'";
				cte4.Comp1 = null;
				cte4.Comp2 = null;
				tx.Commit();
			}

			//Revision 4
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(cte2);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyModFlagProperties()
		{
			Cfg.GetClassMapping("NHibernate.Envers.Tests.Entities.Components.ComponentTestEntity_AUD")
				.ExtractModProperties()
				.Should().Have.SameSequenceAs("Comp1_MOD");
		}

		[Test]
		public void VerifyHasChangedNotAudited()
		{
			Assert.Throws<QueryException>(() =>
			                              QueryForPropertyHasChanged(typeof (ComponentTestEntity), id1, "Comp2")
				);
		}

		[Test]
		public void VerifyHasChangedId1()
		{
			QueryForPropertyHasChanged(typeof (ComponentTestEntity), id1, "Comp1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 2);

			QueryForPropertyHasNotChanged(typeof(ComponentTestEntity), id1, "Comp1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Be.Empty();
		}

		[Test]
		public void VerifyHasChangedId2()
		{
			QueryForPropertyHasChangedWithDeleted(typeof(ComponentTestEntity), id2, "Comp1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 2, 4);

			QueryForPropertyHasNotChangedWithDeleted(typeof(ComponentTestEntity), id2, "Comp1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Be.Empty();
		}

		[Test]
		public void VerifyHasChangedId3()
		{
			QueryForPropertyHasChangedWithDeleted(typeof(ComponentTestEntity), id3, "Comp1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 3);

			QueryForPropertyHasNotChangedWithDeleted(typeof(ComponentTestEntity), id3, "Comp1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Be.Empty();
		}

		[Test]
		public void VerifyHasChangedId4()
		{
			QueryForPropertyHasChangedWithDeleted(typeof(ComponentTestEntity), id4, "Comp1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(2, 3);

			QueryForPropertyHasNotChangedWithDeleted(typeof(ComponentTestEntity), id4, "Comp1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Components.Mapping.hbm.xml" };
			}
		}
	}
}