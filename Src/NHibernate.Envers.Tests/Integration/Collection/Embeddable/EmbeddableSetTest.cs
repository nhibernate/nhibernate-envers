using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Collection;
using NHibernate.Envers.Tests.Entities.Components;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Collection.Embeddable
{
	public class EmbeddableSetTest : TestBase
	{
		private int ese1Id;

		private Component4 c4_1;
		private Component4 c4_2;
		private Component3 c3_1;
		private Component3 c3_2;

		public EmbeddableSetTest(string strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			c4_1 = new Component4 { Key = "c41", Value = "c41_value", Description = "c41_description" };
			c4_2 = new Component4 { Key = "c42", Value = "c42_value2", Description = "c42_description" };
			c3_1 = new Component3 { Str1 = "c31", AuditedComponent = c4_1, NonAuditedComponent = c4_2 };
			c3_2 = new Component3 { Str1 = "c32", AuditedComponent = c4_1, NonAuditedComponent = c4_2 };
			var ese1 = new EmbeddableSetEntity();

			// Revision 1 (ese1: initially 1 element in both collections)
			using (var tx = Session.BeginTransaction())
			{
				ese1.ComponentSet.Add(c3_1);
				ese1Id = (int) Session.Save(ese1);
				tx.Commit();
			}

			// Revision (still 1) (ese1: removing non-existing element)
			using (var tx = Session.BeginTransaction())
			{
				ese1.ComponentSet.Remove(c3_2);
				tx.Commit();
			}

			// Revision 2 (ese1: adding one element)
			using (var tx = Session.BeginTransaction())
			{
				ese1.ComponentSet.Add(c3_2);
				tx.Commit();
			}

			// Revision 3 (ese1: adding one existing element)
			using (var tx = Session.BeginTransaction())
			{
				ese1.ComponentSet.Add(c3_1);
				tx.Commit();
			}

			// Revision 4 (ese1: removing one existing element)
			using (var tx = Session.BeginTransaction())
			{
				ese1.ComponentSet.Remove(c3_2);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			AuditReader().GetRevisions(typeof (EmbeddableSetEntity), ese1Id)
			             .Should().Have.SameSequenceAs(1, 2, 3);
		}

		[Test]
		public void VerifyHistoryOfEse1()
		{
			var rev1 = AuditReader().Find<EmbeddableSetEntity>(ese1Id, 1);
			var rev2 = AuditReader().Find<EmbeddableSetEntity>(ese1Id, 2);
			var rev3 = AuditReader().Find<EmbeddableSetEntity>(ese1Id, 3);

			rev1.ComponentSet.Should().Have.SameValuesAs(c3_1);
			rev2.ComponentSet.Should().Have.SameValuesAs(c3_1, c3_2);
			rev3.ComponentSet.Should().Have.SameValuesAs(c3_1);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Collection.Mapping.hbm.xml"};
			}
		}
	}
}