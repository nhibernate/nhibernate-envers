using NHibernate.Envers.Tests.Integration.Inheritance.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Joined.NotOwnedRelation
{
	public partial class NotOwnBidirectionalTest : TestBase
	{
		private long pc_id;
		private long a1_id;
		private long a2_id;

		public NotOwnBidirectionalTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			pc_id = 1;
			a1_id = 10;
			a2_id = 100;
			var pc = new PersonalContact {Id = pc_id, Email = "e", FirstName = "f"};
			var a1 = new Address {Id = a1_id, Address1 = "a1", Contact = pc};
			var a2 = new Address { Id = a2_id, Address1 = "a2", Contact = pc };
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(pc);
				Session.Save(a1);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(a2);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(Contact), pc_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(PersonalContact), pc_id));

			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(Address), a1_id));
			CollectionAssert.AreEquivalent(new[] { 2 }, AuditReader().GetRevisions(typeof(Address), a2_id));
		}

		[Test]
		public void VerifyHistoryOfContact()
		{
			CollectionAssert.AreEquivalent(new[] {new Address{Id = a1_id, Address1 = "a1"}}, 
							AuditReader().Find<Contact>(pc_id, 1).Addresses);
			CollectionAssert.AreEquivalent(new[] { new Address { Id = a1_id, Address1 = "a1" }, new Address { Id = a2_id, Address1 = "a2" } }, 
							AuditReader().Find<Contact>(pc_id, 2).Addresses);
		}

		[Test]
		public void VerifyHistoryOfPersonalContact()
		{
			CollectionAssert.AreEquivalent(new[] { new Address { Id = a1_id, Address1 = "a1" } },
							AuditReader().Find<PersonalContact>(pc_id, 1).Addresses);
			CollectionAssert.AreEquivalent(new[] { new Address { Id = a1_id, Address1 = "a1" }, new Address { Id = a2_id, Address1 = "a2" } },
							AuditReader().Find<PersonalContact>(pc_id, 2).Addresses);
		}
	}
}