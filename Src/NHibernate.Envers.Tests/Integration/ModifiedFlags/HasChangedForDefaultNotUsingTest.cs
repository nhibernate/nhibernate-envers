using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.Components;
using NHibernate.Envers.Tests.Integration.ModifiedFlags.Entities;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class HasChangedForDefaultNotUsingTest : AbstractModifiedFlagsEntityTest
	{
		private const int entityId = 37;
		private const int refEntityId = 133;

		public HasChangedForDefaultNotUsingTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override bool ForceModifiedFlags
		{
			get { return false; }
		}

		protected override void Initialize()
		{
			var entity = new PartialModifiedFlagsEntity {Id = entityId};

			//revision1
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(entity);
				tx.Commit();
			}

			//Revision2
			using (var tx = Session.BeginTransaction())
			{
				entity.Data = "data1";
				tx.Commit();
			}

			//revision 3
			using (var tx = Session.BeginTransaction())
			{
				entity.Comp1 = new Component1 {Str1 = "str1", Str2 = "Str2"};
				tx.Commit();
			}

			//revision 4
			using (var tx = Session.BeginTransaction())
			{
				entity.Comp2 = new Component2 {Str5 = "str1", Str6 = "str2"};
				tx.Commit();
			}

			//revision 5
			var withModifiedFlagReferencingEntity = new WithModifiedFlagReferencingEntity { Id = refEntityId, Data = "first", Reference = entity };
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(withModifiedFlagReferencingEntity);
				tx.Commit();
			}

			//revision 6
			using (var tx = Session.BeginTransaction())
			{
				withModifiedFlagReferencingEntity.Reference = null;
				withModifiedFlagReferencingEntity.SecondReference = entity;
				tx.Commit();
			}

			//revision 7
			using (var tx = Session.BeginTransaction())
			{
				entity.StringSet.Add("firstelement");
				entity.StringSet.Add("secondelement");
				tx.Commit();
			}

			//revision 8
			using (var tx = Session.BeginTransaction())
			{
				entity.StringSet.Remove("secondelement");
				entity.StringMap["somekey"] = "somevalue";
				tx.Commit();
			}

			// Revision 9 - main entity doesn't change
			var strTestEntity = new StrTestEntity {Str = "first"};
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(strTestEntity);
				tx.Commit();
			}

			// Revision 10
			using (var tx = Session.BeginTransaction())
			{
				entity.EntitiesSet.Add(strTestEntity);
				tx.Commit();
			}

			//revision 11
			using (var tx = Session.BeginTransaction())
			{
				entity.EntitiesSet.Remove(strTestEntity);
				entity.EntitiesMap["somekey"] = strTestEntity;
				tx.Commit();
			}

			// Revision 12 - main entity doesn't change
			using (var tx = Session.BeginTransaction())
			{
				strTestEntity.Str = "second";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionsCounts()
		{
			AuditReader().GetRevisions(typeof (PartialModifiedFlagsEntity), entityId)
				.Should().Have.SameSequenceAs(1, 2, 3, 4, 5, 6, 7, 8, 10, 11);
		}

		[Test]
		public void VerifyHasChangedData()
		{
			QueryForPropertyHasChanged(typeof (PartialModifiedFlagsEntity), entityId, "Data")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(2);
		}

		[Test]
		public void VerifyHasChangedComp1()
		{
			QueryForPropertyHasChanged(typeof(PartialModifiedFlagsEntity), entityId, "Comp1")
					.ExtractRevisionNumbersFromRevision()
					.Should().Have.SameSequenceAs(3);
		}

		[Test]
		public void VerifyHasChangedComp2()
		{
			Assert.Throws<QueryException>(() =>
					QueryForPropertyHasChanged(typeof (PartialModifiedFlagsEntity), entityId, "Comp2")
				);
		}

		[Test]
		public void VerifyHasChangedReferencing()
		{
			QueryForPropertyHasChanged(typeof(PartialModifiedFlagsEntity), entityId, "Referencing")
					.ExtractRevisionNumbersFromRevision()
					.Should().Have.SameSequenceAs(5, 6);			
		}

		[Test]
		public void VerifyHasChangedReferencing2()
		{
			Assert.Throws<QueryException>(() =>
					QueryForPropertyHasChanged(typeof(PartialModifiedFlagsEntity), entityId, "Referencing2")
				);
		}

		[Test]
		public void VerifyHasChangedStringSet()
		{
			QueryForPropertyHasChanged(typeof(PartialModifiedFlagsEntity), entityId, "StringSet")
					.ExtractRevisionNumbersFromRevision()
					.Should().Have.SameSequenceAs(1, 7, 8);				
		}

		[Test]
		public void VerifyHasChangedStringMap()
		{
			QueryForPropertyHasChanged(typeof(PartialModifiedFlagsEntity), entityId, "StringMap")
					.ExtractRevisionNumbersFromRevision()
					.Should().Have.SameSequenceAs(1, 8);
		}

		[Test]
		public void VerifyHasChangedStringSetAndMap()
		{
			QueryForPropertyHasChanged(typeof(PartialModifiedFlagsEntity), entityId, "StringSet", "StringMap")
					.ExtractRevisionNumbersFromRevision()
					.Should().Have.SameSequenceAs(1, 8);
		}

		[Test]
		public void VerifyHasChangedEntitiesSet()
		{
			QueryForPropertyHasChanged(typeof(PartialModifiedFlagsEntity), entityId, "EntitiesSet")
					.ExtractRevisionNumbersFromRevision()
					.Should().Have.SameSequenceAs(1, 10, 11);			
		}

		[Test]
		public void VerifyHasChangedEntitiesMap()
		{
			QueryForPropertyHasChanged(typeof(PartialModifiedFlagsEntity), entityId, "EntitiesMap")
					.ExtractRevisionNumbersFromRevision()
					.Should().Have.SameSequenceAs(1, 11);
		}

		[Test]
		public void VerifyHasChangedEntitiesSetAndMap()
		{
			QueryForPropertyHasChanged(typeof(PartialModifiedFlagsEntity), entityId, "EntitiesSet", "EntitiesMap")
					.ExtractRevisionNumbersFromRevision()
					.Should().Have.SameSequenceAs(1, 11);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml", 
									"Integration.ModifiedFlags.Entities.Mapping.hbm.xml" };
			}
		}
	}
}