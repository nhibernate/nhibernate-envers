using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.Collection;
using NHibernate.Envers.Tests.Entities.ManyToMany;
using NHibernate.Envers.Tests.Entities.ManyToMany.UniDirectional;
using NHibernate.Envers.Tests.Entities.OneToMany;
using NHibernate.Envers.Tests.Integration.ManyToMany.Ternary;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Proxy
{
	public partial class RemovedObjectQueryTest : TestBase
	{
		private int stringSetId;
		private UnversionedStrTestEntity unversionedEntity1;
		private UnversionedStrTestEntity unversionedEntity2;
		private StrTestPrivSeqEntity stringEntity1;
		private StrTestPrivSeqEntity stringEntity2;
		private IntTestPrivSeqEntity intEntity1;
		private IntTestPrivSeqEntity intEntity2;
		private int ternaryMapId;

		public RemovedObjectQueryTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			ConfigurationKey.StoreDataAtDelete.SetUserValue(configuration, true);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[]
					{
						"Entities.ManyToMany.Mapping.hbm.xml", "Entities.OneToMany.Mapping.hbm.xml", "Entities.Collection.Mapping.hbm.xml"
						, "Entities.Mapping.hbm.xml", "Entities.ManyToMany.UniDirectional.Mapping.hbm.xml", "Integration.ManyToMany.Ternary.Mapping.hbm.xml"
					};
			}
		}

		protected override void Initialize()
		{
			var refEdEntity1 = new SetRefEdEntity { Id = 1, Data = "Demo Data 1" };
			var refIngEntity1 = new SetRefIngEntity { Id = 2, Data = "Example Data 1", Reference = refEdEntity1 };

			//Revision 1
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(refEdEntity1);
				Session.Save(refIngEntity1);
				tx.Commit();
			}

			//Revision 2 - removing both object in the same revision
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(refIngEntity1);
				Session.Delete(refEdEntity1);
				tx.Commit();
			}

			var refEdEntity2 = new SetRefEdEntity { Id = 3, Data = "Demo Data 2" };
			var refIngEntity2 = new SetRefIngEntity { Id = 4, Data = "Example Data 2", Reference = refEdEntity2 };

			//Revision 3
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(refEdEntity2);
				Session.Save(refIngEntity2);
				tx.Commit();
			}

			//Revision 4 - removing child object
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(refIngEntity2);
				tx.Commit();
			}

			//Revision 5 - removing parent object
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(refEdEntity2);
				tx.Commit();
			}

			var setOwningEntity1 = new SetOwningEntity { Id = 5, Data = "Demo Data 1" };
			var setOwnedEntity1 = new SetOwnedEntity { Id = 6, Data = "Example Data 1" };
			var owning = new HashSet<SetOwningEntity>();
			var owned = new HashSet<SetOwnedEntity>();
			owning.Add(setOwningEntity1);
			owned.Add(setOwnedEntity1);
			setOwningEntity1.References = owned;
			setOwnedEntity1.Referencing = owning;

			//Revision 6
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(setOwnedEntity1);
				Session.Save(setOwningEntity1);
				tx.Commit();
			}

			//Revision 7 - removing both object in the same revision
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(setOwningEntity1);
				Session.Delete(setOwnedEntity1);
				tx.Commit();
			}

			var setOwningEntity2 = new SetOwningEntity { Id = 7, Data = "Demo Data 2" };
			var setOwnedEntity2 = new SetOwnedEntity { Id = 8, Data = "Example Data 2" };
			owning = new HashSet<SetOwningEntity>();
			owned = new HashSet<SetOwnedEntity>();
			owning.Add(setOwningEntity2);
			owned.Add(setOwnedEntity2);
			setOwningEntity2.References = owned;
			setOwnedEntity2.Referencing = owning;

			//Revision 8
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(setOwnedEntity2);
				Session.Save(setOwningEntity2);
				tx.Commit();
			}

			//Revision 9 - removing first object
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(setOwningEntity2);
				tx.Commit();
			}

			//Revision 10 - removing second object
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(setOwnedEntity2);
				tx.Commit();
			}

			var stringSetEntity = new StringSetEntity();
			stringSetEntity.Strings.Add("string 1");
			stringSetEntity.Strings.Add("string 2");

			//Revision 11
			using (var tx = Session.BeginTransaction())
			{
				stringSetId = (int)Session.Save(stringSetEntity);
				tx.Commit();
			}

			//Revision 12 - removing element collection
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(stringSetEntity);
				tx.Commit();
			}

			unversionedEntity1 = new UnversionedStrTestEntity { Str = "string 1" };
			unversionedEntity2 = new UnversionedStrTestEntity { Str = "string 2" };
			var relationNotAuditedEntity = new M2MIndexedListTargetNotAuditedEntity { Id = 1, Data = "Parent" };
			relationNotAuditedEntity.References.Add(unversionedEntity1);
			relationNotAuditedEntity.References.Add(unversionedEntity2);
			//Revision 13
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(unversionedEntity1);
				Session.Save(unversionedEntity2);
				Session.Save(relationNotAuditedEntity);
				tx.Commit();
			}

			//Revision 14 - removing entity with unversioned relation
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(relationNotAuditedEntity);
				tx.Commit();
			}

			stringEntity1 = new StrTestPrivSeqEntity { Str = "value 1" };
			stringEntity2 = new StrTestPrivSeqEntity { Str = "value 2" };
			intEntity1 = new IntTestPrivSeqEntity { Number = 1 };
			intEntity2 = new IntTestPrivSeqEntity { Number = 2 };
			var mapEntity = new TernaryMapEntity();
			mapEntity.Map[intEntity1] = stringEntity1;
			mapEntity.Map[intEntity2] = stringEntity2;
			//Revision 15
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(stringEntity1);
				Session.Save(stringEntity2);
				Session.Save(intEntity1);
				Session.Save(intEntity2);
				ternaryMapId = (int)Session.Save(mapEntity);
				tx.Commit();
			}

			//Revision 16 - updating ternary map
			using (var tx = Session.BeginTransaction())
			{
				intEntity2.Number = 3;
				stringEntity2.Str = "Value 3";
				tx.Commit();
			}

			// Revision 17 - removing ternary map
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(mapEntity);
				tx.Commit();
			}

			var collEd1 = new CollectionRefEdEntity { Id = 1, Data = "data_ed_1" };
			var collIng1 = new CollectionRefIngEntity { Id = 2, Data = "data_ing_1", Reference = collEd1 };
			collEd1.Reffering = new List<CollectionRefIngEntity> { collIng1 };

			//Revision 18 - testing one-to-many collection
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(collEd1);
				Session.Save(collIng1);
				tx.Commit();
			}

			//Revision 19
			using (var tx = Session.BeginTransaction())
			{
				collIng1.Data = "modified data_ing_1";
				tx.Commit();
			}

			//Revision 20
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(collIng1);
				Session.Delete(collEd1);
				tx.Commit();
			}

			var listEd1 = new ListOwnedEntity { Id = 1, Data = "data_ed_1" };
			var listIng1 = new ListOwningEntity { Id = 2, Data = "data_ing_1" };
			listEd1.Referencing = new List<ListOwningEntity> { listIng1 };
			listIng1.References = new List<ListOwnedEntity> { listEd1 };

			//Revision 21
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(listEd1);
				Session.Save(listIng1);
				tx.Commit();
			}

			//Revision 22
			using (var tx = Session.BeginTransaction())
			{
				listIng1.Data = "modified data_ing_1";
				tx.Commit();
			}

			//Revision 23
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(listIng1);
				Session.Delete(listEd1);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyTernaryMap()
		{
			var ternaryMap = new TernaryMapEntity { Id = ternaryMapId };
			ternaryMap.Map[intEntity1] = stringEntity1;
			ternaryMap.Map[new IntTestPrivSeqEntity { Id = intEntity2.Id, Number = 2 }] = new StrTestPrivSeqEntity
				{
					Id = stringEntity2.Id,
					Str = "value 2"
				};

			var entity = AuditReader().Find<TernaryMapEntity>(ternaryMapId, 15);
			entity.Map.Should().Have.SameValuesAs(ternaryMap.Map);

			ternaryMap.Map.Clear();
			ternaryMap.Map.Add(intEntity1, stringEntity1);
			ternaryMap.Map.Add(intEntity2, stringEntity2);

			entity = AuditReader().Find<TernaryMapEntity>(ternaryMapId, 16);
			entity.Map.Should().Have.SameValuesAs(ternaryMap.Map);

			var res = AuditReader().CreateQuery().ForHistoryOf<TernaryMapEntity, DefaultRevisionEntity>(true)
														 .Add(AuditEntity.Id().Eq(ternaryMapId))
														 .Add(AuditEntity.RevisionType().Eq(RevisionType.Deleted))
														 .Results().First();
			res.RevisionEntity.Id.Should().Be.EqualTo(17);
			res.Entity.Map.Should().Have.SameValuesAs(ternaryMap.Map);
		}

		[Test]
		public void VerifyOneToManyCollectionSemantics()
		{
			var edVer1 = new CollectionRefEdEntity {Id = 1, Data = "data_ed_1"};
			var ingVer1 = new CollectionRefIngEntity {Id = 2, Data = "data_ing_1"};
			var ingVer2 = new CollectionRefIngEntity {Id = 2, Data = "modified data_ing_1"};

			var entity = AuditReader().Find<CollectionRefEdEntity>(1, 18);
			entity.Should().Be.EqualTo(edVer1);
			entity.Reffering.Should().Have.SameValuesAs(ingVer1);

			entity = AuditReader().Find<CollectionRefEdEntity>(1, 19);
			entity.Reffering.Should().Have.SameValuesAs(ingVer2);

			var res = AuditReader().CreateQuery().ForHistoryOf<CollectionRefEdEntity, DefaultRevisionEntity>(true)
			                       .Add(AuditEntity.Id().Eq(1))
			                       .Add(AuditEntity.RevisionType().Eq(RevisionType.Deleted))
			                       .Results().First();
			res.RevisionEntity.Id.Should().Be.EqualTo(20);
			res.Entity.Data.Should().Be.EqualTo("data_ed_1");
			res.Entity.Reffering.Should().Have.SameValuesAs(ingVer2);
		}

		[Test]
		public void VerifyManyToManyCollectionSemantics()
		{
			var edVer1 = new ListOwnedEntity { Id = 1, Data = "data_ed_1" };
			var ingVer1 = new ListOwningEntity { Id = 2, Data = "data_ing_1" };
			var ingVer2 = new ListOwningEntity { Id = 2, Data = "modified data_ing_1" };

			var entity = AuditReader().Find<ListOwnedEntity>(1, 21);
			entity.Should().Be.EqualTo(edVer1);
			entity.Referencing.Should().Have.SameValuesAs(ingVer1);

			entity = AuditReader().Find<ListOwnedEntity>(1, 22);
			entity.Referencing.Should().Have.SameValuesAs(ingVer2);

			var res = AuditReader().CreateQuery().ForHistoryOf<ListOwnedEntity, DefaultRevisionEntity>(true)
														 .Add(AuditEntity.Id().Eq(1))
														 .Add(AuditEntity.RevisionType().Eq(RevisionType.Deleted))
														 .Results().First();
			res.RevisionEntity.Id.Should().Be.EqualTo(23);
			res.Entity.Data.Should().Be.EqualTo("data_ed_1");
			res.Entity.Referencing.Should().Have.SameValuesAs(ingVer2);
		}

		[Test]
		public void VerifyUnversionedRelation()
		{
			var res = AuditReader().CreateQuery().ForHistoryOf<M2MIndexedListTargetNotAuditedEntity, DefaultRevisionEntity>()
														 .Add(AuditEntity.Id().Eq(1))
														 .Add(AuditEntity.RevisionType().Eq(RevisionType.Deleted))
														 .Results().First();
			res.RevisionEntity.Id.Should().Be.EqualTo(14);
			res.Entity.References
				 .Should().Have.SameSequenceAs(unversionedEntity1, unversionedEntity2);
		}

		[Test]
		public void VerifyElementCollection()
		{
			var res = AuditReader().CreateQuery().ForHistoryOf<StringSetEntity, DefaultRevisionEntity>()
											 .Add(AuditEntity.Id().Eq(stringSetId))
											 .Add(AuditEntity.RevisionType().Eq(RevisionType.Deleted))
											 .Results().First();
			res.RevisionEntity.Id.Should().Be.EqualTo(12);
			res.Entity.Strings
				 .Should().Have.SameValuesAs("string 1", "string 2");
		}

		[Test]
		public void VerifyReferencedOneToManySameRevision()
		{
			var res = AuditReader().CreateQuery().ForHistoryOf<SetRefIngEntity, DefaultRevisionEntity>()
								 .Add(AuditEntity.Id().Eq(2))
								 .Add(AuditEntity.RevisionType().Eq(RevisionType.Deleted))
								 .Results().First();
			res.RevisionEntity.Id.Should().Be.EqualTo(2);
			res.Entity.Data.Should().Be.EqualTo("Example Data 1");
			res.Entity.Reference.Data.Should().Be.EqualTo("Demo Data 1");
		}

		[Test]
		public void VerifyReferringOneToManySameRevision()
		{
			var res = AuditReader().CreateQuery().ForHistoryOf<SetRefEdEntity, DefaultRevisionEntity>()
					 .Add(AuditEntity.Id().Eq(1))
					 .Add(AuditEntity.RevisionType().Eq(RevisionType.Deleted))
					 .Results().First();
			res.RevisionEntity.Id.Should().Be.EqualTo(2);
			res.Entity.Data.Should().Be.EqualTo("Demo Data 1");
			res.Entity.Reffering.Should().Have.SameValuesAs(new SetRefIngEntity { Id = 2, Data = "Example Data 1" });
		}

		[Test]
		public void VerifyReferencedOneToManyDifferentRevisions()
		{
			var res = AuditReader().CreateQuery().ForHistoryOf<SetRefIngEntity, DefaultRevisionEntity>()
					 .Add(AuditEntity.Id().Eq(4))
					 .Add(AuditEntity.RevisionType().Eq(RevisionType.Deleted))
					 .Results().First();
			res.RevisionEntity.Id.Should().Be.EqualTo(4);
			res.Entity.Data.Should().Be.EqualTo("Example Data 2");
			res.Entity.Reference.Data.Should().Be.EqualTo("Demo Data 2");
		}

		[Test]
		public void VerifyReferringOneToManyDifferentRevision()
		{
			var res = AuditReader().CreateQuery().ForHistoryOf<SetRefEdEntity, DefaultRevisionEntity>()
					 .Add(AuditEntity.Id().Eq(3))
					 .Add(AuditEntity.RevisionType().Eq(RevisionType.Deleted))
					 .Results().First();
			res.RevisionEntity.Id.Should().Be.EqualTo(5);
			res.Entity.Data.Should().Be.EqualTo("Demo Data 2");
			res.Entity.Reffering.Should().Be.Empty();

			//after commit in revision 4, child entity has been removed
			res = AuditReader().CreateQuery().ForHistoryOf<SetRefEdEntity, DefaultRevisionEntity>()
												 .Add(AuditEntity.Id().Eq(3))
												 .Add(AuditEntity.RevisionNumber().Eq(4))
												 .Results().First();
			res.Entity.Data.Should().Be.EqualTo("Demo Data 2");
			res.Entity.Reffering.Should().Be.Empty();
		}

		[Test]
		public void VerifyOwnedManyToManySameRevision()
		{
			var res = AuditReader().CreateQuery().ForHistoryOf<SetOwningEntity, DefaultRevisionEntity>()
														 .Add(AuditEntity.Id().Eq(5))
														 .Add(AuditEntity.RevisionType().Eq(RevisionType.Deleted))
														 .Results().First();
			res.RevisionEntity.Id.Should().Be.EqualTo(7);
			res.Entity.Data.Should().Be.EqualTo("Demo Data 1");
			res.Entity.References.Should().Have.SameValuesAs(new SetOwnedEntity { Id = 6, Data = "Example Data 1" });
		}

		[Test]
		public void VerifyOwningManyToManySameRevision()
		{
			var res = AuditReader().CreateQuery().ForHistoryOf<SetOwnedEntity, DefaultRevisionEntity>()
														 .Add(AuditEntity.Id().Eq(6))
														 .Add(AuditEntity.RevisionType().Eq(RevisionType.Deleted))
														 .Results().First();
			res.RevisionEntity.Id.Should().Be.EqualTo(7);
			res.Entity.Data.Should().Be.EqualTo("Example Data 1");
			res.Entity.Referencing.Should().Have.SameValuesAs(new SetOwningEntity { Id = 5, Data = "Demo Data 1" });
		}

		[Test]
		public void VerifyOwnedManyToManyDifferentRevisions()
		{
			var res = AuditReader().CreateQuery().ForHistoryOf<SetOwningEntity, DefaultRevisionEntity>()
											 .Add(AuditEntity.Id().Eq(7))
											 .Add(AuditEntity.RevisionType().Eq(RevisionType.Deleted))
											 .Results().First();
			res.RevisionEntity.Id.Should().Be.EqualTo(9);
			res.Entity.Data.Should().Be.EqualTo("Demo Data 2");
			res.Entity.References.Should().Have.SameValuesAs(new SetOwnedEntity { Id = 8, Data = "Example Data 2" });
		}

		[Test]
		public void VerifyOwningManyToManyDifferentRevisions()
		{
			var res = AuditReader().CreateQuery().ForHistoryOf<SetOwnedEntity, DefaultRevisionEntity>()
											 .Add(AuditEntity.Id().Eq(8))
											 .Add(AuditEntity.RevisionType().Eq(RevisionType.Deleted))
											 .Results().First();
			res.RevisionEntity.Id.Should().Be.EqualTo(10);
			res.Entity.Data.Should().Be.EqualTo("Example Data 2");
			res.Entity.Referencing.Should().Be.Empty();

			//after commit in revision 9, related entity has been removed
			res = AuditReader().CreateQuery().ForHistoryOf<SetOwnedEntity, DefaultRevisionEntity>()
														 .Add(AuditEntity.Id().Eq(8))
														 .Add(AuditEntity.RevisionNumber().Eq(9))
														 .Results().First();
			res.Entity.Data.Should().Be.EqualTo("Example Data 2");
			res.Entity.Referencing.Should().Be.Empty();
		}
	}
}