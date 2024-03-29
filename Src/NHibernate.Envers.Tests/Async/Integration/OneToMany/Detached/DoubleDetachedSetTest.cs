﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.OneToMany.Detached;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToMany.Detached
{
	using System.Threading.Tasks;
	public partial class DoubleDetachedSetTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountAsync()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, await (AuditReader().GetRevisionsAsync(typeof(DoubleSetRefCollEntity), coll1_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 1 }, await (AuditReader().GetRevisionsAsync(typeof(StrTestEntity), str1_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 1 }, await (AuditReader().GetRevisionsAsync(typeof(StrTestEntity), str2_id)).ConfigureAwait(false));
		}


		[Test]
		public async Task VerifyHistoryOfColl1Async()
		{
			var str1 = await (Session.GetAsync<StrTestEntity>(str1_id)).ConfigureAwait(false);
			var str2 = await (Session.GetAsync<StrTestEntity>(str2_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<DoubleSetRefCollEntity>(coll1_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<DoubleSetRefCollEntity>(coll1_id, 2)).ConfigureAwait(false);
			var rev3 = await (AuditReader().FindAsync<DoubleSetRefCollEntity>(coll1_id, 3)).ConfigureAwait(false);

			CollectionAssert.AreEquivalent(new[] { str1 }, rev1.Collection);
			CollectionAssert.AreEquivalent(new[] { str1, str2 }, rev2.Collection);
			CollectionAssert.AreEquivalent(new[] { str2 }, rev3.Collection);

			CollectionAssert.AreEquivalent(new[] { str2 }, rev1.Collection2);
			CollectionAssert.AreEquivalent(new[] { str2 }, rev2.Collection2);
			CollectionAssert.AreEquivalent(new[] { str1, str2 }, rev3.Collection2);
		}
	}
}