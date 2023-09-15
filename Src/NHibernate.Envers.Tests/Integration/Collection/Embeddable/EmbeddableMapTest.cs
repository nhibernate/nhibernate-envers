using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Collection;
using NHibernate.Envers.Tests.Entities.Components;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Collection.Embeddable
{
	public partial class EmbeddableMapTest : TestBase
	{
		private int eme1Id;
		private int eme2Id;

		private Component4 c4_1;
		private Component4 c4_2;
		private Component3 c3_1;
		private Component3 c3_2;

		public EmbeddableMapTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			c4_1 = new Component4 { Key = "c41", Value = "c41_value", Description = "c41_description" };
			c4_2 = new Component4 { Key = "c42", Value = "c42_value2", Description = "c42_description" };
			c3_1 = new Component3 { Str1 = "c31", AuditedComponent = c4_1, NonAuditedComponent = c4_2 };
			c3_2 = new Component3 { Str1 = "c32", AuditedComponent = c4_1, NonAuditedComponent = c4_2 };

			var eme1 = new EmbeddableMapEntity();
			var eme2 = new EmbeddableMapEntity();

			// Revision 1 (eme1: initialy empty, eme2: initialy 1 mapping)
			using (var tx = Session.BeginTransaction())
			{
				eme2.ComponentMap["1"] = c3_1;
				eme1Id = (int) Session.Save(eme1);
				eme2Id = (int) Session.Save(eme2);
				tx.Commit();
			}

			// Revision 2 (eme1: adding 2 mappings, eme2: no changes)
			using (var tx = Session.BeginTransaction())
			{
				eme1.ComponentMap["1"] = c3_1;
				eme1.ComponentMap["2"] = c3_2;
				tx.Commit();
			}

			// Revision 3 (eme1: removing an existing mapping, eme2: replacing a value)
			using (var tx = Session.BeginTransaction())
			{
				eme1.ComponentMap.Remove("1");
				eme2.ComponentMap["1"] = c3_2;
				tx.Commit();
			}

			// No revision (eme1: removing a non-existing mapping, eme2: replacing with the same value)
			using (var tx = Session.BeginTransaction())
			{
				eme1.ComponentMap.Remove("3");
				eme2.ComponentMap["1"] = c3_2;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			AuditReader().GetRevisions(typeof(EmbeddableMapEntity), eme1Id)
									 .Should().Have.SameSequenceAs(1, 2, 3);
			AuditReader().GetRevisions(typeof(EmbeddableMapEntity), eme2Id)
									 .Should().Have.SameSequenceAs(1, 3);
		}

		[Test]
		public void VerifyHistoryOfEme1()
		{
			var rev1 = AuditReader().Find<EmbeddableMapEntity>(eme1Id, 1);
			var rev2 = AuditReader().Find<EmbeddableMapEntity>(eme1Id, 2);
			var rev3 = AuditReader().Find<EmbeddableMapEntity>(eme1Id, 3);
			var rev4 = AuditReader().Find<EmbeddableMapEntity>(eme1Id, 4);

			rev1.ComponentMap.Should().Be.Empty();
			rev2.ComponentMap.Should().Have.SameValuesAs(new KeyValuePair<string, Component3>("1", c3_1), new KeyValuePair<string, Component3>("2", c3_2));
			rev3.ComponentMap.Should().Have.SameValuesAs(new KeyValuePair<string, Component3>("2", c3_2));
			rev4.ComponentMap.Should().Have.SameValuesAs(new KeyValuePair<string, Component3>("2", c3_2));
		}

		[Test]
		public void VerifyHistoryOfEme2()
		{
			var rev1 = AuditReader().Find<EmbeddableMapEntity>(eme2Id, 1);
			var rev2 = AuditReader().Find<EmbeddableMapEntity>(eme2Id, 2);
			var rev3 = AuditReader().Find<EmbeddableMapEntity>(eme2Id, 3);
			var rev4 = AuditReader().Find<EmbeddableMapEntity>(eme2Id, 4);


			rev1.ComponentMap.Should().Have.SameValuesAs(new KeyValuePair<string, Component3>("1", c3_1));
			rev2.ComponentMap.Should().Have.SameValuesAs(new KeyValuePair<string, Component3>("1", c3_1));
			rev3.ComponentMap.Should().Have.SameValuesAs(new KeyValuePair<string, Component3>("1", c3_2));
			rev4.ComponentMap.Should().Have.SameValuesAs(new KeyValuePair<string, Component3>("1", c3_2));
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Collection.Mapping.hbm.xml" };
			}
		}
	}
}