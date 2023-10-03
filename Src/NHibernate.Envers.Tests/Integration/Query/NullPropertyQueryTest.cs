using System.Collections.Generic;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.Ids;
using NHibernate.Envers.Tests.Entities.OneToMany;
using NHibernate.Envers.Tests.Entities.OneToMany.Ids;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Query
{
	public partial class NullPropertyQueryTest : TestBase
	{
		private int idSimplePropertyNull;
		private int idSimplePropertyNotNull;
		private readonly EmbId idMulticolumnReferenceToParentNull = new EmbId { X = 0, Y = 1 };
		private const int idReferenceToParentNotNull = 1;
		private const int idParent = 1;

		public NullPropertyQueryTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			//rev 1
			using (var tx = Session.BeginTransaction())
			{
				var nullSite = new StrIntTestEntity{Number=1, Str = null};
				var notNullSite = new StrIntTestEntity { Number = 2, Str = "data" };
				idSimplePropertyNull = (int) Session.Save(nullSite);
				idSimplePropertyNotNull = (int) Session.Save(notNullSite);
				tx.Commit();
			}
			//rev 2
			using (var tx = Session.BeginTransaction())
			{
				var nullParentSrieie = new SetRefIngEmbIdEntity{Data = "data", Id= idMulticolumnReferenceToParentNull, Reference = null};
				Session.Save(nullParentSrieie);
				tx.Commit();
			}
			//rev 3
			using (var tx = Session.BeginTransaction())
			{
				var parent = new CollectionRefEdEntity{ Id = idParent, Data = "data"};
				var notNullParentCrie = new CollectionRefIngEntity{Id = idReferenceToParentNotNull, Data = "data", Reference = parent};
				Session.Save(parent);
				Session.Save(notNullParentCrie);
				tx.Commit();
			}
		}

		[Test]
		public void SimplePropertyIsNullQuery()
		{
			var ver = AuditReader().CreateQuery()
				.ForEntitiesAtRevision<StrIntTestEntity>(1)
				.Add(AuditEntity.Property("Str").IsNull())
				.Single();
			ver.Should().Be.EqualTo(new StrIntTestEntity {Id = idSimplePropertyNull, Number = 1, Str = null});
		}

		[Test]
		public void SimplePropertyIsNotNullQuery()
		{
			var ver = AuditReader().CreateQuery()
				.ForEntitiesAtRevision<StrIntTestEntity>(1)
				.Add(AuditEntity.Property("Str").IsNotNull())
				.Single();
			ver.Should().Be.EqualTo(new StrIntTestEntity { Id = idSimplePropertyNotNull, Number = 2, Str = "data" });
		}

		[Test]
		public void ReferenceMulticolumnPropertyIsNullQuery()
		{
			var ver = AuditReader().CreateQuery()
				.ForEntitiesAtRevision<SetRefIngEmbIdEntity>(2)
				.Add(AuditEntity.Property("Reference").IsNull())
				.Single();
			ver.Id.Should().Be.EqualTo(idMulticolumnReferenceToParentNull);
		}

		[Test]
		public void ReferencePropertyIsNotNullQuery()
		{
			var ver = AuditReader().CreateQuery()
				.ForEntitiesAtRevision<CollectionRefIngEntity>(3)
				.Add(AuditEntity.Property("Reference").IsNotNull())
				.Single();
			ver.Id.Should().Be.EqualTo(idReferenceToParentNotNull);
		}
		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml", "Entities.OneToMany.Ids.Mapping.hbm.xml", "Entities.OneToMany.Mapping.hbm.xml" };
			}
		}
	}
}