using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Collection;
using NHibernate.Envers.Tests.Entities.Components;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class HasChangedComponentCollectionTest : AbstractModifiedFlagsEntityTest
	{
		private int ele1Id;

		private Component4 c4_1;
		private Component4 c4_2;
		private Component3 c3_1;
		private Component3 c3_2;


		public HasChangedComponentCollectionTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			c4_1 = new Component4 { Key = "c41", Value = "c41_value", Description = "c41_description" };
			c4_2 = new Component4 { Key = "c42", Value = "c42_value2", Description = "c42_description" };
			c3_1 = new Component3 { Str1 = "c31", AuditedComponent = c4_1, NonAuditedComponent = c4_2 };
			c3_2 = new Component3 { Str1 = "c32", AuditedComponent = c4_1, NonAuditedComponent = c4_2 };
			var ele1 = new EmbeddableListEntity1{OtherData = "data", ComponentList = new List<Component3>{c3_1}};

			//Revision 1 (ele1: initially 1 element in both collections)
			using (var tx = Session.BeginTransaction())
			{
				ele1Id = (int) Session.Save(ele1);
				tx.Commit();
			}

			//Revision (still 1) (ele1: removing non-existing element)
			using (var tx = Session.BeginTransaction())
			{
				ele1.ComponentList.Remove(c3_2);
				tx.Commit();
			}

			//Revision 2 (ele1: updating singular property and removing non-existing element)
			using (var tx = Session.BeginTransaction())
			{
				ele1.OtherData = "modified";
				ele1.ComponentList.Remove(c3_2);
				tx.Commit();
			}

			// Revision 3 (ele1: adding one element)
			using (var tx = Session.BeginTransaction())
			{
				ele1.ComponentList.Add(c3_2);
				tx.Commit();
			}

			// Revision 4 (ele1: adding one existing element)
			using (var tx = Session.BeginTransaction())
			{
				ele1.ComponentList.Add(c3_1);
				tx.Commit();
			}

			// Revision 5 (ele1: removing one existing element)
			using (var tx = Session.BeginTransaction())
			{
				ele1.ComponentList.Remove(c3_2);
				tx.Commit();
			}

			// Revision 6 (ele1: changing singular property only)
			using (var tx = Session.BeginTransaction())
			{
				ele1.OtherData = "another modification";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyHasChangedEle()
		{
			QueryForPropertyHasChanged(typeof (EmbeddableListEntity1), ele1Id, "ComponentList")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 3, 4, 5);

			QueryForPropertyHasChanged(typeof (EmbeddableListEntity1), ele1Id, "OtherData")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 2, 6);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Collection.Mapping.hbm.xml", "Entities.Mapping.hbm.xml" };
			}
		}
	}
}