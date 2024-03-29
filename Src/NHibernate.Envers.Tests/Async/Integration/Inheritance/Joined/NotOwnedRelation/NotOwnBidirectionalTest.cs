﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NHibernate.Envers.Tests.Integration.Inheritance.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Joined.NotOwnedRelation
{
	using System.Threading.Tasks;
	public partial class NotOwnBidirectionalTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountAsync()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, await (AuditReader().GetRevisionsAsync(typeof(Contact), pc_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, await (AuditReader().GetRevisionsAsync(typeof(PersonalContact), pc_id)).ConfigureAwait(false));

			CollectionAssert.AreEquivalent(new[] { 1 }, await (AuditReader().GetRevisionsAsync(typeof(Address), a1_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 2 }, await (AuditReader().GetRevisionsAsync(typeof(Address), a2_id)).ConfigureAwait(false));
		}

		[Test]
		public async Task VerifyHistoryOfContactAsync()
		{
			CollectionAssert.AreEquivalent(new[] {new Address{Id = a1_id, Address1 = "a1"}}, 
							(await (AuditReader().FindAsync<Contact>(pc_id, 1)).ConfigureAwait(false)).Addresses);
			CollectionAssert.AreEquivalent(new[] { new Address { Id = a1_id, Address1 = "a1" }, new Address { Id = a2_id, Address1 = "a2" } }, 
							(await (AuditReader().FindAsync<Contact>(pc_id, 2)).ConfigureAwait(false)).Addresses);
		}

		[Test]
		public async Task VerifyHistoryOfPersonalContactAsync()
		{
			CollectionAssert.AreEquivalent(new[] { new Address { Id = a1_id, Address1 = "a1" } },
							(await (AuditReader().FindAsync<PersonalContact>(pc_id, 1)).ConfigureAwait(false)).Addresses);
			CollectionAssert.AreEquivalent(new[] { new Address { Id = a1_id, Address1 = "a1" }, new Address { Id = a2_id, Address1 = "a2" } },
							(await (AuditReader().FindAsync<PersonalContact>(pc_id, 2)).ConfigureAwait(false)).Addresses);
		}
	}
}