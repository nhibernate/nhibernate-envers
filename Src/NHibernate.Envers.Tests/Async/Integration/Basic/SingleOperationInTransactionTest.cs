﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using NHibernate.Envers.Exceptions;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Basic
{
	using System.Threading.Tasks;
	public partial class SingleOperationInTransactionTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountsAsync()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 4, 6 }, await (AuditReader().GetRevisionsAsync(typeof(BasicTestEntity1), id1)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 2, 5, 7 }, await (AuditReader().GetRevisionsAsync(typeof(BasicTestEntity1), id2)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 3 }, await (AuditReader().GetRevisionsAsync(typeof(BasicTestEntity1), id3)).ConfigureAwait(false));
		}

		[Test]
		public async Task VerifyHistoryOf1Async()
		{
			var ver1 = new BasicTestEntity1 { Id = id1, Str1 = "x", Long1 = 1 };
			var ver2 = new BasicTestEntity1 { Id = id1, Str1 = "x2", Long1 = 2 };
			var ver3 = new BasicTestEntity1 { Id = id1, Str1 = "x3", Long1 = 3 };

			Assert.AreEqual(ver1, await (AuditReader().FindAsync<BasicTestEntity1>(id1, 1)).ConfigureAwait(false));
			Assert.AreEqual(ver1, await (AuditReader().FindAsync<BasicTestEntity1>(id1, 2)).ConfigureAwait(false));
			Assert.AreEqual(ver1, await (AuditReader().FindAsync<BasicTestEntity1>(id1, 3)).ConfigureAwait(false));
			Assert.AreEqual(ver2, await (AuditReader().FindAsync<BasicTestEntity1>(id1, 4)).ConfigureAwait(false));
			Assert.AreEqual(ver2, await (AuditReader().FindAsync<BasicTestEntity1>(id1, 5)).ConfigureAwait(false));
			Assert.AreEqual(ver3, await (AuditReader().FindAsync<BasicTestEntity1>(id1, 6)).ConfigureAwait(false));
			Assert.AreEqual(ver3, await (AuditReader().FindAsync<BasicTestEntity1>(id1, 7)).ConfigureAwait(false));
		}

		[Test]
		public async Task VerifyHistoryOf2Async()
		{
			var ver1 = new BasicTestEntity1 { Id = id2, Str1 = "y", Long1 = 20 };
			var ver2 = new BasicTestEntity1 { Id = id2, Str1 = "y2", Long1 = 20 };
			var ver3 = new BasicTestEntity1 { Id = id2, Str1 = "y3", Long1 = 21 };

			Assert.IsNull(await (AuditReader().FindAsync<BasicTestEntity1>(id2, 1)).ConfigureAwait(false));
			Assert.AreEqual(ver1, await (AuditReader().FindAsync<BasicTestEntity1>(id2, 2)).ConfigureAwait(false));
			Assert.AreEqual(ver1, await (AuditReader().FindAsync<BasicTestEntity1>(id2, 3)).ConfigureAwait(false));
			Assert.AreEqual(ver1, await (AuditReader().FindAsync<BasicTestEntity1>(id2, 4)).ConfigureAwait(false));
			Assert.AreEqual(ver2, await (AuditReader().FindAsync<BasicTestEntity1>(id2, 5)).ConfigureAwait(false));
			Assert.AreEqual(ver2, await (AuditReader().FindAsync<BasicTestEntity1>(id2, 6)).ConfigureAwait(false));
			Assert.AreEqual(ver3, await (AuditReader().FindAsync<BasicTestEntity1>(id2, 7)).ConfigureAwait(false));
		}

		[Test]
		public async Task VerifyHistoryOf3Async()
		{
			var ver1 = new BasicTestEntity1 { Id = id3, Str1 = "z", Long1 = 30 };

			Assert.IsNull(await (AuditReader().FindAsync<BasicTestEntity1>(id3, 1)).ConfigureAwait(false));
			Assert.IsNull(await (AuditReader().FindAsync<BasicTestEntity1>(id3, 2)).ConfigureAwait(false));
			Assert.AreEqual(ver1, await (AuditReader().FindAsync<BasicTestEntity1>(id3, 3)).ConfigureAwait(false));
			Assert.AreEqual(ver1, await (AuditReader().FindAsync<BasicTestEntity1>(id3, 4)).ConfigureAwait(false));
			Assert.AreEqual(ver1, await (AuditReader().FindAsync<BasicTestEntity1>(id3, 5)).ConfigureAwait(false));
			Assert.AreEqual(ver1, await (AuditReader().FindAsync<BasicTestEntity1>(id3, 6)).ConfigureAwait(false));
			Assert.AreEqual(ver1, await (AuditReader().FindAsync<BasicTestEntity1>(id3, 7)).ConfigureAwait(false));
		}

		[Test]
		public async Task VerifyHistoryOfNonExistingEntityAsync()
		{
			Assert.IsNull(await (AuditReader().FindAsync<BasicTestEntity1>(id1 + id2 + id3, 1)).ConfigureAwait(false));
			Assert.IsNull(await (AuditReader().FindAsync<BasicTestEntity1>(id1 + id2 + id3, 7)).ConfigureAwait(false));
		}
	}
}