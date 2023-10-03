using System.Collections.Generic;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Entities.OneToMany;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Query.RelationIn
{
	public partial class RelationInTest : TestBase
	{
		private SetRefIngEntity ing;
		private SetRefEdEntity ed;
		private SetRefEdEntity edNotReferenced;

		public RelationInTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.OneToMany.Mapping.hbm.xml", "Entities.Mapping.hbm.xml" };
			}
		}
		protected override void Initialize()
		{
			ed = new SetRefEdEntity {Data = "foo", Id = 17};
			ing = new SetRefIngEntity {Data = "bar", Reference = ed, Id =34};
			edNotReferenced = new SetRefEdEntity {Data = "baz", Id = 4};
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ed);
				Session.Save(ing);
				Session.Save(edNotReferenced);
				tx.Commit();
			}
		}

		[Test]
		public void ShouldBeAbleToUseInOnSingleSide()
		{
			AuditReader().CreateQuery().ForRevisionsOf<SetRefIngEntity>()
				.Add(AuditEntity.Property("Reference").In(new List<SetRefEdEntity> { ed , edNotReferenced }))
				.Add(AuditEntity.Property("Data").Eq("bar"))
				.Results().Should().Contain(ing);
		}

		[Test]
		public void ShouldThrowUsingInOnCollection()
		{
			Assert.Throws<AuditException>(() =>
			               AuditReader().CreateQuery().ForRevisionsOf<SetRefEdEntity>()
			                  .Add(AuditEntity.Property("Reffering").In(new List<SetRefIngEntity> {ing}))
			                  .Results());
		}
	}
}