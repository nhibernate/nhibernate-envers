﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.OneToMany.Detached;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToMany.Detached
{
	using System.Threading.Tasks;
	public partial class IndexedJoinColumnBidirectionalListTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountAsync()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4 }, await (AuditReader().GetRevisionsAsync(typeof(IndexedListJoinColumnBidirectionalRefIngEntity), ing1_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 4 }, await (AuditReader().GetRevisionsAsync(typeof(IndexedListJoinColumnBidirectionalRefIngEntity), ing2_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 1, 3, 4 }, await (AuditReader().GetRevisionsAsync(typeof(IndexedListJoinColumnBidirectionalRefEdEntity), ed1_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 4 }, await (AuditReader().GetRevisionsAsync(typeof(IndexedListJoinColumnBidirectionalRefEdEntity), ed2_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4 }, await (AuditReader().GetRevisionsAsync(typeof(IndexedListJoinColumnBidirectionalRefEdEntity), ed3_id)).ConfigureAwait(false));
		}

		[Test]
		public async Task VerifyHistoryOfIng1Async()
		{
			var ed1 = await (Session.GetAsync<IndexedListJoinColumnBidirectionalRefEdEntity>(ed1_id)).ConfigureAwait(false);
			var ed2 = await (Session.GetAsync<IndexedListJoinColumnBidirectionalRefEdEntity>(ed2_id)).ConfigureAwait(false);
			var ed3 = await (Session.GetAsync<IndexedListJoinColumnBidirectionalRefEdEntity>(ed3_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<IndexedListJoinColumnBidirectionalRefIngEntity>(ing1_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<IndexedListJoinColumnBidirectionalRefIngEntity>(ing1_id, 2)).ConfigureAwait(false);
			var rev3 = await (AuditReader().FindAsync<IndexedListJoinColumnBidirectionalRefIngEntity>(ing1_id, 3)).ConfigureAwait(false);
			var rev4 = await (AuditReader().FindAsync<IndexedListJoinColumnBidirectionalRefIngEntity>(ing1_id, 4)).ConfigureAwait(false);

			CollectionAssert.AreEqual(new[] { ed1, ed2, ed3 }, rev1.References);
			CollectionAssert.AreEqual(new[] { ed1, ed3 }, rev2.References);
			CollectionAssert.AreEqual(new[] { ed3, ed1 }, rev3.References);
			CollectionAssert.AreEqual(new[] { ed2, ed3, ed1 }, rev4.References);
		}

		[Test]
		public async Task VerifyHistoryOfIng2Async()
		{
			var ed2 = await (Session.GetAsync<IndexedListJoinColumnBidirectionalRefEdEntity>(ed2_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<IndexedListJoinColumnBidirectionalRefIngEntity>(ing2_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<IndexedListJoinColumnBidirectionalRefIngEntity>(ing2_id, 2)).ConfigureAwait(false);
			var rev3 = await (AuditReader().FindAsync<IndexedListJoinColumnBidirectionalRefIngEntity>(ing2_id, 3)).ConfigureAwait(false);
			var rev4 = await (AuditReader().FindAsync<IndexedListJoinColumnBidirectionalRefIngEntity>(ing2_id, 4)).ConfigureAwait(false);

			CollectionAssert.IsEmpty(rev1.References);
			CollectionAssert.AreEqual(new[] { ed2 }, rev2.References);
			CollectionAssert.AreEqual(new[] { ed2 }, rev3.References);
			CollectionAssert.IsEmpty(rev4.References);
		}

		[Test]
		public async Task VerifyHistoryOfEd1Async()
		{
			var ing1 = await (Session.GetAsync<IndexedListJoinColumnBidirectionalRefIngEntity>(ing1_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<IndexedListJoinColumnBidirectionalRefEdEntity>(ed1_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<IndexedListJoinColumnBidirectionalRefEdEntity>(ed1_id, 2)).ConfigureAwait(false);
			var rev3 = await (AuditReader().FindAsync<IndexedListJoinColumnBidirectionalRefEdEntity>(ed1_id, 3)).ConfigureAwait(false);
			var rev4 = await (AuditReader().FindAsync<IndexedListJoinColumnBidirectionalRefEdEntity>(ed1_id, 4)).ConfigureAwait(false);

			Assert.AreEqual(ing1, rev1.Owner);
			Assert.AreEqual(ing1, rev2.Owner);
			Assert.AreEqual(ing1, rev3.Owner);
			Assert.AreEqual(ing1, rev4.Owner);

			Assert.AreEqual(0, rev1.Position);
			Assert.AreEqual(0, rev2.Position);
			Assert.AreEqual(1, rev3.Position);
			Assert.AreEqual(2, rev4.Position);
		}

		[Test]
		public async Task VerifyHistoryOfEd2Async()
		{
			var ing1 = await (Session.GetAsync<IndexedListJoinColumnBidirectionalRefIngEntity>(ing1_id)).ConfigureAwait(false);
			var ing2 = await (Session.GetAsync<IndexedListJoinColumnBidirectionalRefIngEntity>(ing2_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<IndexedListJoinColumnBidirectionalRefEdEntity>(ed2_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<IndexedListJoinColumnBidirectionalRefEdEntity>(ed2_id, 2)).ConfigureAwait(false);
			var rev3 = await (AuditReader().FindAsync<IndexedListJoinColumnBidirectionalRefEdEntity>(ed2_id, 3)).ConfigureAwait(false);
			var rev4 = await (AuditReader().FindAsync<IndexedListJoinColumnBidirectionalRefEdEntity>(ed2_id, 4)).ConfigureAwait(false);

			Assert.AreEqual(ing1, rev1.Owner);
			Assert.AreEqual(ing2, rev2.Owner);
			Assert.AreEqual(ing2, rev3.Owner);
			Assert.AreEqual(ing1, rev4.Owner);

			Assert.AreEqual(1, rev1.Position);
			Assert.AreEqual(0, rev2.Position);
			Assert.AreEqual(0, rev3.Position);
			Assert.AreEqual(0, rev4.Position);
		}

		[Test]
		public async Task VerifyHistoryOfEd3Async()
		{
			var ing1 = await (Session.GetAsync<IndexedListJoinColumnBidirectionalRefIngEntity>(ing1_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<IndexedListJoinColumnBidirectionalRefEdEntity>(ed3_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<IndexedListJoinColumnBidirectionalRefEdEntity>(ed3_id, 2)).ConfigureAwait(false);
			var rev3 = await (AuditReader().FindAsync<IndexedListJoinColumnBidirectionalRefEdEntity>(ed3_id, 3)).ConfigureAwait(false);
			var rev4 = await (AuditReader().FindAsync<IndexedListJoinColumnBidirectionalRefEdEntity>(ed3_id, 4)).ConfigureAwait(false);

			Assert.AreEqual(ing1, rev1.Owner);
			Assert.AreEqual(ing1, rev2.Owner);
			Assert.AreEqual(ing1, rev3.Owner);
			Assert.AreEqual(ing1, rev4.Owner);

			Assert.AreEqual(2, rev1.Position);
			Assert.AreEqual(1, rev2.Position);
			Assert.AreEqual(0, rev3.Position);
			Assert.AreEqual(1, rev4.Position);
		}
	}
}