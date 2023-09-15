using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Collection;
using NHibernate.Envers.Tests.Entities.Components;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Collection.Embeddable
{
	public partial class EmbeddableList1Test : TestBase
	{
		private int ele1Id;
		private Component4 c4_1;
		private Component4 c4_2;
		private Component3 c3_1;
		private Component3 c3_2;

		public EmbeddableList1Test(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			c4_1 = new Component4 { Key = "c41", Value = "c41_value", Description = "c41_description" };
			c4_2 = new Component4 { Key = "c42", Value = "c42_value2", Description = "c42_description" };
			c3_1 = new Component3 { Str1 = "c31", AuditedComponent = c4_1, NonAuditedComponent = c4_2 };
			c3_2 = new Component3 { Str1 = "c32", AuditedComponent = c4_1, NonAuditedComponent = c4_2 };

			var ele1 = new EmbeddableListEntity1();
			// Revision 1 (ele1: initially 1 element in both collections)
			using (var tx = Session.BeginTransaction())
			{
				ele1.ComponentList.Add(c3_1);
				ele1Id = (int) Session.Save(ele1);
				tx.Commit();
			}

			// Revision (still 1) (ele1: removing non-existing element)
			using (var tx = Session.BeginTransaction())
			{
				ele1.ComponentList.Remove(c3_2);
				tx.Commit();
			}

			// Revision 2 (ele1: adding one element)
			using (var tx = Session.BeginTransaction())
			{
				ele1.ComponentList.Add(c3_2);
				tx.Commit();
			}

			// Revision 3 (ele1: adding one existing element)
			using (var tx = Session.BeginTransaction())
			{
				ele1.ComponentList.Add(c3_1);
				tx.Commit();
			}

			// Revision 4 (ele1: removing one existing element)
			using (var tx = Session.BeginTransaction())
			{
				ele1.ComponentList.Remove(c3_2);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			AuditReader().GetRevisions(typeof (EmbeddableListEntity1), ele1Id)
			             .Should().Have.SameSequenceAs(1, 2, 3, 4);
		}

		[Test]
		public void VerifyHistoryOfEle1()
		{
			var rev1 = AuditReader().Find<EmbeddableListEntity1>(ele1Id, 1);
			var rev2 = AuditReader().Find<EmbeddableListEntity1>(ele1Id, 2);
			var rev3 = AuditReader().Find<EmbeddableListEntity1>(ele1Id, 3);
			var rev4 = AuditReader().Find<EmbeddableListEntity1>(ele1Id, 4);

			rev1.ComponentList.Should().Have.SameSequenceAs(c3_1);
			rev2.ComponentList.Should().Have.SameSequenceAs(c3_1, c3_2);
			rev3.ComponentList.Should().Have.SameSequenceAs(c3_1, c3_2, c3_1);
			rev4.ComponentList.Should().Have.SameSequenceAs(c3_1, c3_1);
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Collection.Mapping.hbm.xml" }; }
		}
	}
}