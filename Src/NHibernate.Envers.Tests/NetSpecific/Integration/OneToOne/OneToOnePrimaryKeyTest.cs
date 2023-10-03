using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.OneToOne
{
	public partial class OneToOnePrimaryKeyTest : TestBase
	{
		private const int id = 47;

		public OneToOnePrimaryKeyTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var owning = new OneToOneOwningEntity{Id=id};
			var owned1 = new OneToOneOwnedEntity { Data = "1"};
			var owned2 = new OneToOneOwnedEntity { Data = "2"};
			//revision 1 - owning with no owned
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(owning);
				tx.Commit();
			}
			//revision 2 - setting owned 1
			using (var tx = Session.BeginTransaction())
			{
				owned1.Owning = owning;
				Session.Save(owned1);
				tx.Commit();
			}
			//revision 3 - setting owned 2
			using (var tx = Session.BeginTransaction())
			{
				owned2.Owning = owning;
				Session.Delete(owned1);
				Session.Save(owned2);
				tx.Commit();
			}
			//revision 4 - no owned
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(owned2); //actually the same as owned1 in db
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(OneToOneOwningEntity), id).ToList());
			CollectionAssert.AreEquivalent(new[] { 2, 3, 4 }, AuditReader().GetRevisions(typeof(OneToOneOwnedEntity), id).ToList());
		}

		[Test]
		public void VerifyHistoryOfOwning()
		{
			var owned1 = new OneToOneOwnedEntity{Id=id, Data="1"};
			var owned2 = new OneToOneOwnedEntity{Id=id, Data="2"};

			AuditReader().Find<OneToOneOwningEntity>(id, 1).Owned
				.Should().Be.Null();
			AuditReader().Find<OneToOneOwningEntity>(id, 2).Owned
				.Should().Be.EqualTo(owned1);
			AuditReader().Find<OneToOneOwningEntity>(id, 3).Owned
				.Should().Be.EqualTo(owned2);
			AuditReader().Find<OneToOneOwningEntity>(id, 4).Owned
				.Should().Be.Null();
		}

		[Test]
		public void VerifyHistoryOfOwned()
		{
			var owning = Session.Get<OneToOneOwningEntity>(id);

			AuditReader().Find<OneToOneOwnedEntity>(id, 1).Should().Be.Null();

			var ver2 = AuditReader().Find<OneToOneOwnedEntity>(id, 2);
			ver2.Data.Should().Be.EqualTo("1");
			ver2.Owning.Should().Be.EqualTo(owning);
			
			var ver3 = AuditReader().Find<OneToOneOwnedEntity>(id, 3);
			ver3.Data.Should().Be.EqualTo("2");
			ver3.Owning.Should().Be.EqualTo(owning);
			
			AuditReader().Find<OneToOneOwnedEntity>(id, 4).Should().Be.Null();
		}
	}
}