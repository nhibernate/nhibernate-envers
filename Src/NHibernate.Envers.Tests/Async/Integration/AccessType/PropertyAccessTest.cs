﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.AccessType
{
	using System.Threading.Tasks;
	public partial class PropertyAccessTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountAsync()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, await (AuditReader().GetRevisionsAsync(typeof(PropertyAccessEntity), id)).ConfigureAwait(false));
		}

		[Test]
		public async Task VerifyHistoryAsync()
		{
			Assert.AreEqual("first", (await (AuditReader().FindAsync<PropertyAccessEntity>(id, 1)).ConfigureAwait(false)).Data);
			Assert.AreEqual("second", (await (AuditReader().FindAsync<PropertyAccessEntity>(id, 2)).ConfigureAwait(false)).Data);
		}
	}
}