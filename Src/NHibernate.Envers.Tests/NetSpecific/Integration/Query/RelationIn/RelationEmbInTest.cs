using System.Collections.Generic;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Entities.Ids;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Query.RelationIn
{
	public partial class RelationEmbInTest : TestBase
	{
		private EmbIdTestEntity embEnt;
		private EmbIdTestEntity embEntNoRef1;
		private EmbIdTestEntity embEntNoRef2;
		private EntityReferingEmbEntity ent;

		public RelationEmbInTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "NetSpecific.Integration.Query.RelationIn.Mapping.hbm.xml", "Entities.Ids.Mapping.hbm.xml" };
			}
		}

		protected override void Initialize()
		{
			embEnt = new EmbIdTestEntity{Id = new EmbId{X = 3,Y = 4}};
			ent = new EntityReferingEmbEntity { Data = "data", Reference = embEnt };
			embEntNoRef1 = new EmbIdTestEntity { Id = new EmbId { X = 4, Y = 3 } };
			embEntNoRef2 = new EmbIdTestEntity { Id = new EmbId { X = 12, Y = 15 } };

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(embEnt);
				Session.Save(ent);
				Session.Save(embEntNoRef1);
				Session.Save(embEntNoRef2);
				tx.Commit();
			}
		}

		[Test]
		public void ShouldFind()
		{
			AuditReader().CreateQuery().ForRevisionsOf<EntityReferingEmbEntity>()
				.Add(AuditEntity.Property("Data").Eq("data"))
				.Add(AuditEntity.Property("Reference").In(new List<EmbIdTestEntity> { embEntNoRef1, embEnt, embEntNoRef2 }))
				.Results().Should().Have.SameSequenceAs(new[]{ent});
		}

		[Test]
		public void ShouldMiss()
		{
			AuditReader().CreateQuery().ForRevisionsOf<EntityReferingEmbEntity>()
				.Add(AuditEntity.Property("Data").Eq("data"))
				.Add(AuditEntity.Property("Reference").In(new List<EmbIdTestEntity> { embEntNoRef1, embEntNoRef2 }))
				.Results().Should().Be.Empty();
		}

		[Test]
		public void ShouldMissDueToOtherProoerty()
		{
			AuditReader().CreateQuery().ForRevisionsOf<EntityReferingEmbEntity>()
				.Add(AuditEntity.Property("Reference").In(new List<EmbIdTestEntity> { embEnt }))
				.Add(AuditEntity.Property("Data").Eq("not data"))
				.Results().Should().Be.Empty();
		}
	}
}