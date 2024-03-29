﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NHibernate.Envers.Tests.Entities.Ids;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToOne.BiDirectional.Ids
{
	using System.Threading.Tasks;
	public partial class EmbIdBidirectionalTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountAsync()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, await (AuditReader().GetRevisionsAsync(typeof(BiEmbIdRefEdEntity), ed1_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, await (AuditReader().GetRevisionsAsync(typeof(BiEmbIdRefEdEntity), ed2_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, await (AuditReader().GetRevisionsAsync(typeof(BiEmbIdRefIngEntity), ing1_id)).ConfigureAwait(false));
		}

		[Test]
		public async Task VerifyHistoryOfEd1Async()
		{
			var ing1 = await (Session.GetAsync<BiEmbIdRefIngEntity>(ing1_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<BiEmbIdRefEdEntity>(ed1_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<BiEmbIdRefEdEntity>(ed1_id, 2)).ConfigureAwait(false);

			Assert.AreEqual(ing1, rev1.Referencing);
			Assert.IsNull(rev2.Referencing);
		}

		[Test]
		public async Task VerifyHistoryOfEd2Async()
		{
			var ing1 = await (Session.GetAsync<BiEmbIdRefIngEntity>(ing1_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<BiEmbIdRefEdEntity>(ed2_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<BiEmbIdRefEdEntity>(ed2_id, 2)).ConfigureAwait(false);

			Assert.IsNull(rev1.Referencing);
			Assert.AreEqual(ing1, rev2.Referencing);
		}

		[Test]
		public async Task VerifyHistoryOfIng1Async()
		{
			var ed1 = await (Session.GetAsync<BiEmbIdRefEdEntity>(ed1_id)).ConfigureAwait(false);
			var ed2 = await (Session.GetAsync<BiEmbIdRefEdEntity>(ed2_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<BiEmbIdRefIngEntity>(ing1_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<BiEmbIdRefIngEntity>(ing1_id, 2)).ConfigureAwait(false);

			Assert.AreEqual(ed1, rev1.Reference);
			Assert.AreEqual(ed2, rev2.Reference);
		}
	}
}