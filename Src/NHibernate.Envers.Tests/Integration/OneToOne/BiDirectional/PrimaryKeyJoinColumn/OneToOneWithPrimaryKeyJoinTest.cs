using NHibernate.Envers.Query;
using NHibernate.Proxy;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.OneToOne.BiDirectional.PrimaryKeyJoinColumn
{
	public partial class OneToOneWithPrimaryKeyJoinTest : TestBase
	{
		private long personId;
		private long accountId;
		private long accountNotAuditedOwnersId;
		private long noProxyPersonId;
		private long proxyPersonId;

		public OneToOneWithPrimaryKeyJoinTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			//Revision 1
			using (var tx = Session.BeginTransaction())
			{
				var person = new Person {Name = "Robert"};
				var account = new Account {Type = "Saving"};
				person.Account = account;
				account.Owner = person;
				personId = (long)Session.Save(person);
				accountId = (long)Session.Save(account);
				tx.Commit();
			}
			//Revision 2
			using (var tx = Session.BeginTransaction())
			{
				var noProxyPerson = new NotAuditedNoProxyPerson {Name = "Kinga"};
				var proxyPerson = new NotAuditedProxyPerson {Name = "Lukasz"};
				var accountNotAuditedOwners = new AccountNotAuditedOwners {Type = "Standard"};
				noProxyPerson.Account = accountNotAuditedOwners;
				proxyPerson.Account = accountNotAuditedOwners;
				accountNotAuditedOwners.Owner = noProxyPerson;
				accountNotAuditedOwners.CoOwner = proxyPerson;
				accountNotAuditedOwnersId = (long)Session.Save(accountNotAuditedOwners);
				noProxyPersonId = (long) Session.Save(noProxyPerson);
				proxyPersonId = (long)Session.Save(proxyPerson);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionsCount()
		{
			AuditReader().GetRevisions(typeof(Person), personId)
				.Should().Have.SameSequenceAs(1);
			AuditReader().GetRevisions(typeof(Account), accountId)
				.Should().Have.SameSequenceAs(1);
			AuditReader().GetRevisions(typeof(AccountNotAuditedOwners), accountNotAuditedOwnersId)
				.Should().Have.SameSequenceAs(2);
		}

		[Test]
		public void VerifyHistoryOfPerson()
		{
			var personVer1 = new Person {PersonId = personId, Name = "Robert"};
			var accountVer1 = new Account {AccountId = accountId, Type = "Saving"};
			personVer1.Account = accountVer1;
			accountVer1.Owner = personVer1;

			var result = (object[])AuditReader().CreateQuery().ForRevisionsOfEntity(typeof (Person), false, true)
								.Add(AuditEntity.Id().Eq(personId))
								.GetResultList()[0];
			result[0].Should().Be.EqualTo(personVer1);
			((Person) result[0]).Account.Should().Be.EqualTo(accountVer1);
			result[2].Should().Be.EqualTo(RevisionType.Added);
			AuditReader().Find<Person>(personId, 1).Should().Be.EqualTo(personVer1);
		}

		[Test]
		public void VerifyHistoryOfAccount()
		{
			var personVer1 = new Person { PersonId = personId, Name = "Robert" };
			var accountVer1 = new Account { AccountId = accountId, Type = "Saving" };
			personVer1.Account = accountVer1;
			accountVer1.Owner = personVer1;

			var result = (object[])AuditReader().CreateQuery().ForRevisionsOfEntity(typeof(Account), false, true)
								.Add(AuditEntity.Id().Eq(accountId))
								.GetResultList()[0];
			result[0].Should().Be.EqualTo(accountVer1);
			((Account)result[0]).Owner.Should().Be.EqualTo(accountVer1.Owner);
			result[2].Should().Be.EqualTo(RevisionType.Added);
			AuditReader().Find<Account>(accountId, 1).Should().Be.EqualTo(accountVer1);
		}

		[Test]
		public void VerifyHistoryOfAccountNotAuditedOwners()
		{
			var noProxyPersonVer1 = new NotAuditedNoProxyPerson {PersonId = noProxyPersonId, Name = "Kinga"};
			var proxyPersonVer1 = new NotAuditedProxyPerson {PersonId = proxyPersonId, Name = "Lukasz"};
			var accountNotAuditedOwnersVer1 = new AccountNotAuditedOwners {AccountId = accountNotAuditedOwnersId, Type = "Standard"};
			noProxyPersonVer1.Account = accountNotAuditedOwnersVer1;
			proxyPersonVer1.Account = accountNotAuditedOwnersVer1;
			accountNotAuditedOwnersVer1.Owner = noProxyPersonVer1;
			accountNotAuditedOwnersVer1.CoOwner = proxyPersonVer1;

			var result = (object[]) AuditReader().CreateQuery().ForRevisionsOfEntity(typeof (AccountNotAuditedOwners), false, true)
				           	.Add(AuditEntity.Id().Eq(accountNotAuditedOwnersId))
				           	.GetResultList()[0];

			var theResult = (AccountNotAuditedOwners) result[0];

			result[0].Should().Be.EqualTo(accountNotAuditedOwnersVer1);
			result[2].Should().Be.EqualTo(RevisionType.Added);
			// Checking non-proxy reference
			theResult.Owner.Should().Be.EqualTo(accountNotAuditedOwnersVer1.Owner);
			// checking proxy reference
			(theResult.CoOwner is INHibernateProxy).Should().Be.True();
			theResult.CoOwner.PersonId.Should().Be.EqualTo(proxyPersonVer1.PersonId);

			AuditReader().Find(typeof (AccountNotAuditedOwners), accountNotAuditedOwnersId, 2)
				.Should().Be.EqualTo(accountNotAuditedOwnersVer1);
		}
	}
}