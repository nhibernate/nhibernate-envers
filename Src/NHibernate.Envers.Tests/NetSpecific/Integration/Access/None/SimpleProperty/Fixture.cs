using System.Collections;
using NHibernate.Envers.Query;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Access.None.SimpleProperty
{
	public partial class Fixture : TestBase
	{
		private int id;

		public Fixture(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ent = new Entity{Data=1};

			using (var tx = Session.BeginTransaction())
			{
				id = (int) Session.Save(ent);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ent.Data = 2;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			CollectionAssert.AreEqual(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(Entity), id));
		}

		[Test]
		public void VerifyHistoryOfComponent()
		{
			var ver1 = AuditReader().Find<Entity>(id, 1);
			var ver2 = AuditReader().Find<Entity>(id, 2);

			ver1.Data.Should().Be.EqualTo(1);

			ver2.Data.Should().Be.EqualTo(2);
		}

		[Test]
		public void CanQueryOnAccessNoneData()
		{
			var res = (IList)AuditReader().CreateQuery()
				.ForRevisionsOfEntity(typeof (Entity), false, false)
				.Add(AuditEntity.Property("Data2").Eq(2))
				.GetSingleResult();

			var ent = (Entity) res[0];
			var rev = (DefaultRevisionEntity) res[1];

			rev.Id.Should().Be.EqualTo(2);
			ent.Data.Should().Be.EqualTo(2);
		}

		[Test]
		public void CanQueryOnAccessNoopData()
		{
			var res = (IList)AuditReader().CreateQuery()
				.ForRevisionsOfEntity(typeof(Entity), false, false)
				.Add(AuditEntity.Property("Data3").Eq(2))
				.GetSingleResult();

			var ent = (Entity)res[0];
			var rev = (DefaultRevisionEntity)res[1];

			rev.Id.Should().Be.EqualTo(2);
			ent.Data.Should().Be.EqualTo(2);
		}
	}
}