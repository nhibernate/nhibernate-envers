using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Merge
{
	public partial class AddDelTest : TestBase
	{
		private const int id = 17;

		public AddDelTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var entity = new GivenIdStrEntity { Id = id, Data = "data" };

			//revision 1
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(entity);
				tx.Commit();
			}
			Session.Clear();

			//revision 2
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(new StrTestEntity {Str = "another data"}); // Just to create second revision
				Session.Delete(entity); // First try to remove the entity
				Session.Save(entity); // Then save it
				tx.Commit();
			}
			Session.Clear();

			//revision 3
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(entity); //First try to remove the entity.
				entity.Data = "modified data"; // Then change it's state.
				Session.Save(entity); //Finally save it
				tx.Commit();
			}
		}

		[Test]
		public void ShouldNotHaveCreatedRevisionForEntityInRevision2()
		{
			AuditReader().GetRevisions(typeof(GivenIdStrEntity), id)
				.Should().Have.SameSequenceAs(1, 3);
		}

		[Test]
		public void ShouldHaveCorrectHistory()
		{
			AuditReader().Find<GivenIdStrEntity>(id, 1)
				.Should().Be.EqualTo(new GivenIdStrEntity {Id = id, Data = "data"});
			AuditReader().Find<GivenIdStrEntity>(id, 3)
				.Should().Be.EqualTo(new GivenIdStrEntity { Id = id, Data = "modified data" });
		}


		protected override IEnumerable<string> Mappings
		{
			get { return new[] {"Entities.Mapping.hbm.xml", "Integration.Merge.Mapping.hbm.xml"}; }
		}
	}
}