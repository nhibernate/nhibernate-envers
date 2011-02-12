using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.OneToOne
{
	[TestFixture, Ignore("Does not work right now")]
	public class OneToOnePrimaryKeyTest : TestBase
	{
		private int id;

		protected override void Initialize()
		{
			var owning = new OneToOneOwningEntity();
			var owned = new OneToOneOwnedEntity();
			using (var tx = Session.BeginTransaction())
			{
				id = (int)Session.Save(owning);
				tx.Commit();
			}
			//revision 2
			using (var tx = Session.BeginTransaction())
			{
				owned.Owning = owning;
				Session.Save(owned);
				tx.Commit();
			}
			//revision 3
			using (var tx = Session.BeginTransaction())
			{
				owning.Owned = null;
				Session.Delete(owned);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, AuditReader().GetRevisions(typeof(OneToOneOwningEntity), id));
			CollectionAssert.AreEquivalent(new[] { 2, 3 }, AuditReader().GetRevisions(typeof(OneToOneOwnedEntity), id));
		}

		[Test]
		public void VerifyHistoryOfOwning()
		{
			AuditReader().Find<OneToOneOwningEntity>(id, 1).Owned
				.Should().Be.Null();
			AuditReader().Find<OneToOneOwningEntity>(id, 2).Owned
				.Should().Not.Be.Null();
			AuditReader().Find<OneToOneOwningEntity>(id, 3).Owned
				.Should().Be.Null();
		}

		[Test]
		public void VerifyHistoryOfOwned()
		{
			AuditReader().Find<OneToOneOwnedEntity>(id, 2).Owning
				.Should().Not.Be.Null();
			AuditReader().Find<OneToOneOwnedEntity>(id, 3)
				.Should().Be.Null();
		}
	}
}