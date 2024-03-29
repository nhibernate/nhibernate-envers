﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Data
{
	using System.Threading.Tasks;
	public partial class DateTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountAsync()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, await (AuditReader().GetRevisionsAsync(typeof(DateTestEntity), id1)).ConfigureAwait(false));
		}

		[Test]
		public async Task VerifyHistoryOfId1Async()
		{
			var ver1 = new DateTestEntity {Id = id1, Date = new DateTime(2000,1,2,3,4,5)};
			var ver2 = new DateTestEntity { Id = id1, Date = new DateTime(2001,2,3,4,5,6) };

			Assert.AreEqual(ver1, await (AuditReader().FindAsync<DateTestEntity>(id1, 1)).ConfigureAwait(false));
			Assert.AreEqual(ver2, await (AuditReader().FindAsync<DateTestEntity>(id1, 2)).ConfigureAwait(false));
		}
	}
}