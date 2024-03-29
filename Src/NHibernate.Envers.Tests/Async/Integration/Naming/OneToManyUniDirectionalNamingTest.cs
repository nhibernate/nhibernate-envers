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
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Naming
{
	using System.Threading.Tasks;
	public partial class OneToManyUniDirectionalNamingTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountAsync()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, await (AuditReader().GetRevisionsAsync(typeof(DetachedNamingTestEntity), uni1_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 1 }, await (AuditReader().GetRevisionsAsync(typeof(StrTestEntity), str1_id)).ConfigureAwait(false));
		}

		[Test]
		public async Task VerifyHistoryOfUniId1Async()
		{
			var str1 = await (Session.GetAsync<StrTestEntity>(str1_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<DetachedNamingTestEntity>(uni1_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<DetachedNamingTestEntity>(uni1_id, 2)).ConfigureAwait(false);

			CollectionAssert.IsEmpty(rev1.Collection);
			CollectionAssert.AreEquivalent(new[] { str1}, rev2.Collection);
			Assert.AreEqual("data1", rev1.Data);
			Assert.AreEqual("data1", rev2.Data);
		}
	}
}