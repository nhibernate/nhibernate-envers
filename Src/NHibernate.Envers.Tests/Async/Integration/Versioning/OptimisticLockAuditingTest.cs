﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Versioning
{
	using System.Threading.Tasks;
	public partial class OptimisticLockAuditingTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountAsync()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, await (AuditReader().GetRevisionsAsync(typeof(OptimisticLockEntity), id)).ConfigureAwait(false));
		}

		[Test]
		public async Task VerifyHistoryAsync()
		{
			var ver1 = new OptimisticLockEntity { Id = id, Str = "X" };
			var ver2 = new OptimisticLockEntity { Id = id, Str = "Y" };

			Assert.AreEqual(ver1, await (AuditReader().FindAsync<OptimisticLockEntity>(id, 1)).ConfigureAwait(false));
			Assert.AreEqual(ver2, await (AuditReader().FindAsync<OptimisticLockEntity>(id, 2)).ConfigureAwait(false));
		}

		[Test]
		public async Task VerifyVersionedIsAuditedAsync()
		{
			var versionOf1 = (await (AuditReader().FindAsync<OptimisticLockEntity>(id, 1)).ConfigureAwait(false)).Version;
			var versionOf2 = (await (AuditReader().FindAsync<OptimisticLockEntity>(id, 2)).ConfigureAwait(false)).Version;

			Assert.AreEqual(versionOf1 +1, versionOf2);
		}
	}
}