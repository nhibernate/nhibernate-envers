﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToMany
{
	using System.Threading.Tasks;
	public partial class BidirectionalMapKeyTest : TestBase
	{


		[Test]
		public async Task VerifyRevisionCountAsync()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, await (AuditReader().GetRevisionsAsync(typeof(RefEdMapKeyEntity), ed_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 1 }, await (AuditReader().GetRevisionsAsync(typeof(RefIngMapKeyEntity), ing1_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, await (AuditReader().GetRevisionsAsync(typeof(RefIngMapKeyEntity), ing2_id)).ConfigureAwait(false));
		}

		[Test]
		public async Task VerifyHistoryOfEdAsync()
		{
			var ing1 = await (Session.GetAsync<RefIngMapKeyEntity>(ing1_id)).ConfigureAwait(false);
			var ing2 = await (Session.GetAsync<RefIngMapKeyEntity>(ing2_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<RefEdMapKeyEntity>(ed_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<RefEdMapKeyEntity>(ed_id, 2)).ConfigureAwait(false);

			CollectionAssert.AreEquivalent(new Dictionary<string, RefIngMapKeyEntity> { { "a", ing1 } }, rev1.IdMap);
			CollectionAssert.AreEquivalent(new Dictionary<string, RefIngMapKeyEntity> { { "a", ing1 }, { "b", ing2 } }, rev2.IdMap);
		}
	}
}