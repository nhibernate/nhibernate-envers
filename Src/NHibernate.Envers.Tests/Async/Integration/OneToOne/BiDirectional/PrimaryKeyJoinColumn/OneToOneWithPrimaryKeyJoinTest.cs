﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NHibernate.Envers.Query;
using NHibernate.Proxy;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.OneToOne.BiDirectional.PrimaryKeyJoinColumn
{
	using System.Threading.Tasks;
	public partial class OneToOneWithPrimaryKeyJoinTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionsCountAsync()
		{
			(await (AuditReader().GetRevisionsAsync(typeof(Person), personId)).ConfigureAwait(false))
				.Should().Have.SameSequenceAs(1);
			(await (AuditReader().GetRevisionsAsync(typeof(Account), accountId)).ConfigureAwait(false))
				.Should().Have.SameSequenceAs(1);
			(await (AuditReader().GetRevisionsAsync(typeof(AccountNotAuditedOwners), accountNotAuditedOwnersId)).ConfigureAwait(false))
				.Should().Have.SameSequenceAs(2);
		}

		[Test]
		public async Task VerifyHistoryOfPersonAsync()
		{
			var personVer1 = new Person {PersonId = personId, Name = "Robert"};
			var accountVer1 = new Account {AccountId = accountId, Type = "Saving"};
			personVer1.Account = accountVer1;
			accountVer1.Owner = personVer1;

			var result = (object[])(await (AuditReader().CreateQuery().ForRevisionsOfEntity(typeof (Person), false, true)
								.Add(AuditEntity.Id().Eq(personId))
								.GetResultListAsync()).ConfigureAwait(false))[0];
			result[0].Should().Be.EqualTo(personVer1);
			((Person) result[0]).Account.Should().Be.EqualTo(accountVer1);
			result[2].Should().Be.EqualTo(RevisionType.Added);
			(await (AuditReader().FindAsync<Person>(personId, 1)).ConfigureAwait(false)).Should().Be.EqualTo(personVer1);
		}

		[Test]
		public async Task VerifyHistoryOfAccountAsync()
		{
			var personVer1 = new Person { PersonId = personId, Name = "Robert" };
			var accountVer1 = new Account { AccountId = accountId, Type = "Saving" };
			personVer1.Account = accountVer1;
			accountVer1.Owner = personVer1;

			var result = (object[])(await (AuditReader().CreateQuery().ForRevisionsOfEntity(typeof(Account), false, true)
								.Add(AuditEntity.Id().Eq(accountId))
								.GetResultListAsync()).ConfigureAwait(false))[0];
			result[0].Should().Be.EqualTo(accountVer1);
			((Account)result[0]).Owner.Should().Be.EqualTo(accountVer1.Owner);
			result[2].Should().Be.EqualTo(RevisionType.Added);
			(await (AuditReader().FindAsync<Account>(accountId, 1)).ConfigureAwait(false)).Should().Be.EqualTo(accountVer1);
		}

		[Test]
		public async Task VerifyHistoryOfAccountNotAuditedOwnersAsync()
		{
			var noProxyPersonVer1 = new NotAuditedNoProxyPerson {PersonId = noProxyPersonId, Name = "Kinga"};
			var proxyPersonVer1 = new NotAuditedProxyPerson {PersonId = proxyPersonId, Name = "Lukasz"};
			var accountNotAuditedOwnersVer1 = new AccountNotAuditedOwners {AccountId = accountNotAuditedOwnersId, Type = "Standard"};
			noProxyPersonVer1.Account = accountNotAuditedOwnersVer1;
			proxyPersonVer1.Account = accountNotAuditedOwnersVer1;
			accountNotAuditedOwnersVer1.Owner = noProxyPersonVer1;
			accountNotAuditedOwnersVer1.CoOwner = proxyPersonVer1;

			var result = (object[]) (await (AuditReader().CreateQuery().ForRevisionsOfEntity(typeof (AccountNotAuditedOwners), false, true)
				           	.Add(AuditEntity.Id().Eq(accountNotAuditedOwnersId))
				           	.GetResultListAsync()).ConfigureAwait(false))[0];

			var theResult = (AccountNotAuditedOwners) result[0];

			result[0].Should().Be.EqualTo(accountNotAuditedOwnersVer1);
			result[2].Should().Be.EqualTo(RevisionType.Added);
			// Checking non-proxy reference
			theResult.Owner.Should().Be.EqualTo(accountNotAuditedOwnersVer1.Owner);
			// checking proxy reference
			(theResult.CoOwner is INHibernateProxy).Should().Be.True();
			theResult.CoOwner.PersonId.Should().Be.EqualTo(proxyPersonVer1.PersonId);

			(await (AuditReader().FindAsync(typeof (AccountNotAuditedOwners), accountNotAuditedOwnersId, 2)).ConfigureAwait(false))
				.Should().Be.EqualTo(accountNotAuditedOwnersVer1);
		}
	}
}