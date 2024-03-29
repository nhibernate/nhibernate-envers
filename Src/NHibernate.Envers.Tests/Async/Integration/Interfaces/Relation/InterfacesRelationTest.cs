﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Interfaces.Relation
{
	using System.Threading.Tasks;
	public partial class InterfacesRelationTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountAsync()
		{
			CollectionAssert.AreEquivalent(new[] { 1 }, await (AuditReader().GetRevisionsAsync(typeof(SetRefEdEntity), ed1_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 1 }, await (AuditReader().GetRevisionsAsync(typeof(SetRefEdEntity), ed2_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 2, 3 }, await (AuditReader().GetRevisionsAsync(typeof(SetRefIngEntity), ing1_id)).ConfigureAwait(false));
		}

		[Test]
		public async Task VerifyHistoryOfEdIng1Async()
		{
			var ed1 = await (Session.GetAsync<SetRefEdEntity>(ed1_id)).ConfigureAwait(false);
			var ed2 = await (Session.GetAsync<SetRefEdEntity>(ed2_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<SetRefIngEntity>(ing1_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<SetRefIngEntity>(ing1_id, 2)).ConfigureAwait(false);
			var rev3 = await (AuditReader().FindAsync<SetRefIngEntity>(ing1_id, 3)).ConfigureAwait(false);

			Assert.IsNull(rev1);
			Assert.AreEqual(ed1, rev2.Reference);
			Assert.AreEqual(ed2, rev3.Reference);
		}
	}
}