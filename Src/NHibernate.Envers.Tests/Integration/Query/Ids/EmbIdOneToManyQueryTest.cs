using System.Collections.Generic;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Entities.Ids;
using NHibernate.Envers.Tests.Entities.OneToMany.Ids;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Query.Ids
{
	public partial class EmbIdOneToManyQueryTest : TestBase
	{
		private EmbId id1;
		private EmbId id2;
		private EmbId id3;
		private EmbId id4;

		public EmbIdOneToManyQueryTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[]{"Entities.Ids.Mapping.hbm.xml", "Entities.OneToMany.Ids.Mapping.hbm.xml"};
			}
		}

		protected override void Initialize()
		{
			id1 = new EmbId {X = 0, Y = 1};
			id2 = new EmbId { X = 10, Y = 11 };
			id3 = new EmbId { X = 20, Y = 21 };
			id4 = new EmbId { X = 30, Y = 31 };

			var refIng1 = new SetRefIngEmbIdEntity {Id = id1, Data = "x"};
			var refIng2 = new SetRefIngEmbIdEntity { Id = id2, Data = "y" };
			var refEd3 = new SetRefEdEmbIdEntity {Id = id3, Data = "a"};
			var refEd4 = new SetRefEdEmbIdEntity { Id = id4, Data = "a" };

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(refIng1);
				Session.Save(refIng2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(refEd3);
				Session.Save(refEd4);
				refIng1.Reference = refEd3;
				refIng2.Reference = refEd4;
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				refIng2.Reference = refEd3;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyEntitiesReferencedToId3()
		{
			var rev1_related = AuditReader().CreateQuery()
				.ForEntitiesAtRevision(typeof (SetRefIngEmbIdEntity), 1)
				.Add(AuditEntity.RelatedId("Reference").Eq(id3))
				.GetResultList();
			var rev1 = AuditReader().CreateQuery()
				.ForEntitiesAtRevision(typeof(SetRefIngEmbIdEntity), 1)
				.Add(AuditEntity.Property("Reference").Eq(new SetRefEdEmbIdEntity {Id = id3}))
				.GetResultList();

			var rev2_related = AuditReader().CreateQuery()
				.ForEntitiesAtRevision(typeof(SetRefIngEmbIdEntity), 2)
				.Add(AuditEntity.RelatedId("Reference").Eq(id3))
				.GetResultList();
			var rev2 = AuditReader().CreateQuery()
				.ForEntitiesAtRevision(typeof(SetRefIngEmbIdEntity), 2)
				.Add(AuditEntity.Property("Reference").Eq(new SetRefEdEmbIdEntity { Id = id3 }))
				.GetResultList();

			var rev3_related = AuditReader().CreateQuery()
				.ForEntitiesAtRevision(typeof(SetRefIngEmbIdEntity), 3)
				.Add(AuditEntity.RelatedId("Reference").Eq(id3))
				.GetResultList();
			var rev3 = AuditReader().CreateQuery()
				.ForEntitiesAtRevision(typeof(SetRefIngEmbIdEntity), 3)
				.Add(AuditEntity.Property("Reference").Eq(new SetRefEdEmbIdEntity { Id = id3 }))
				.GetResultList();

			Assert.AreEqual(rev1, rev1_related);
			Assert.AreEqual(rev2, rev2_related);
			Assert.AreEqual(rev3, rev3_related);

			CollectionAssert.IsEmpty(rev1);
			CollectionAssert.AreEquivalent(new[]
			                               	{
			                               		new SetRefIngEmbIdEntity { Id = id1, Data = "x" }
			                               	}, rev2);
			CollectionAssert.AreEquivalent(new[]
			                               	{
			                               		new SetRefIngEmbIdEntity { Id = id1, Data = "x" }, 
												new SetRefIngEmbIdEntity{ Id = id2, Data = "y" }
			                               	}, rev3);
		}

		[Test]
		public void VerifyEntitiesReferencedToId4()
		{
			var rev1_related = AuditReader().CreateQuery()
				.ForEntitiesAtRevision(typeof(SetRefIngEmbIdEntity), 1)
				.Add(AuditEntity.RelatedId("Reference").Eq(id4))
				.GetResultList();
			var rev2_related = AuditReader().CreateQuery()
				.ForEntitiesAtRevision(typeof(SetRefIngEmbIdEntity), 2)
				.Add(AuditEntity.RelatedId("Reference").Eq(id4))
				.GetResultList();
			var rev3_related = AuditReader().CreateQuery()
				.ForEntitiesAtRevision(typeof(SetRefIngEmbIdEntity), 3)
				.Add(AuditEntity.RelatedId("Reference").Eq(id4))
				.GetResultList();

			CollectionAssert.IsEmpty(rev1_related);
			CollectionAssert.AreEquivalent(new[]{new SetRefIngEmbIdEntity{ Id = id2, Data = "y" }}, rev2_related);
			CollectionAssert.IsEmpty(rev3_related);
		}

		[Test]
		public void VerifyEntitiesReferencedByIng1ToId3()
		{
			var rev1_related = AuditReader().CreateQuery()
				.ForEntitiesAtRevision(typeof(SetRefIngEmbIdEntity), 1)
				.Add(AuditEntity.RelatedId("Reference").Eq(id3))
				.Add(AuditEntity.Id().Eq(id1))
				.GetResultList();
			var rev2_related = AuditReader().CreateQuery()
				.ForEntitiesAtRevision(typeof (SetRefIngEmbIdEntity), 2)
				.Add(AuditEntity.RelatedId("Reference").Eq(id3))
				.Add(AuditEntity.Id().Eq(id1))
				.GetResultList();
			var rev3_related = AuditReader().CreateQuery()
				.ForEntitiesAtRevision(typeof (SetRefIngEmbIdEntity), 3)
				.Add(AuditEntity.RelatedId("Reference").Eq(id3))
				.Add(AuditEntity.Id().Eq(id1))
				.GetResultList();
			CollectionAssert.IsEmpty(rev1_related);
			CollectionAssert.AreEquivalent(new[] { new SetRefIngEmbIdEntity { Id = id1, Data = "x" } }, rev2_related);
			CollectionAssert.AreEquivalent(new[] { new SetRefIngEmbIdEntity { Id = id1, Data = "x" } }, rev3_related);
		}

		[Test]
		public void VerifyEntitiesReferencedByIng2ToId3()
		{
			var rev1_related = AuditReader().CreateQuery()
				.ForEntitiesAtRevision(typeof(SetRefIngEmbIdEntity), 1)
				.Add(AuditEntity.RelatedId("Reference").Eq(id3))
				.Add(AuditEntity.Id().Eq(id2))
				.GetResultList();
			var rev2_related = AuditReader().CreateQuery()
				.ForEntitiesAtRevision(typeof(SetRefIngEmbIdEntity), 2)
				.Add(AuditEntity.RelatedId("Reference").Eq(id3))
				.Add(AuditEntity.Id().Eq(id2))
				.GetResultList();
			var rev3_related = AuditReader().CreateQuery()
				.ForEntitiesAtRevision(typeof(SetRefIngEmbIdEntity), 3)
				.Add(AuditEntity.RelatedId("Reference").Eq(id3))
				.Add(AuditEntity.Id().Eq(id2))
				.GetResultList();
			CollectionAssert.IsEmpty(rev1_related);
			CollectionAssert.IsEmpty(rev2_related);
			CollectionAssert.AreEquivalent(new[] { new SetRefIngEmbIdEntity { Id = id2, Data = "y" } }, rev3_related);
		}
	}
}