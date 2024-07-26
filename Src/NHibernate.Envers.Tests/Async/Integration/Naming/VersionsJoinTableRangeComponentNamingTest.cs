﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Components;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Naming
{
	using System.Threading.Tasks;
	public partial class VersionsJoinTableRangeComponentNamingTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountAsync()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, await (AuditReader().GetRevisionsAsync(typeof(VersionsJoinTableRangeComponentTestEntity), vjrcte_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 2 }, await (AuditReader().GetRevisionsAsync(typeof(VersionsJoinTableRangeTestEntity), vjtrte_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 2 }, await (AuditReader().GetRevisionsAsync(typeof(VersionsJoinTableRangeTestAlternateEntity), vjtrtae_id1)).ConfigureAwait(false));
		}

		[Test]
		public async Task VerifyHistoryUfUniId1Async()
		{
			var vjtrte = await (Session.GetAsync<VersionsJoinTableRangeTestEntity>(vjtrte_id)).ConfigureAwait(false);
			var vjtrtae = await (Session.GetAsync<VersionsJoinTableRangeTestAlternateEntity>(vjtrtae_id1)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<VersionsJoinTableRangeComponentTestEntity>(vjrcte_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<VersionsJoinTableRangeComponentTestEntity>(vjrcte_id, 2)).ConfigureAwait(false);

			CollectionAssert.IsEmpty(rev1.Component1.Range);
			CollectionAssert.IsEmpty(rev1.Component2.Range);
			CollectionAssert.AreEquivalent(new[] { vjtrte }, rev2.Component1.Range);
			CollectionAssert.AreEquivalent(new[] { vjtrtae }, rev2.Component2.Range);
		}
	}
}