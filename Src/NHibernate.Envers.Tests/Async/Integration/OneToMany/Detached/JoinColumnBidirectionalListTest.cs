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
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.OneToMany.Detached
{
	using System.Threading.Tasks;
	public partial class JoinColumnBidirectionalListTest :TestBase
	{

		[Test]
		public async Task VerifyRevisionCountAsync()
		{
			(await (AuditReader().GetRevisionsAsync(typeof(ListJoinColumnBidirectionalRefIngEntity), ing1_id)).ConfigureAwait(false))
				.Should().Have.SameSequenceAs(1, 2, 4);
			(await (AuditReader().GetRevisionsAsync(typeof(ListJoinColumnBidirectionalRefIngEntity), ing2_id)).ConfigureAwait(false))
				.Should().Have.SameSequenceAs(1, 2, 4);
			(await (AuditReader().GetRevisionsAsync(typeof(ListJoinColumnBidirectionalRefEdEntity), ed1_id)).ConfigureAwait(false))
				.Should().Have.SameSequenceAs(1, 3, 4);
			(await (AuditReader().GetRevisionsAsync(typeof(ListJoinColumnBidirectionalRefEdEntity), ed2_id)).ConfigureAwait(false))
				.Should().Have.SameSequenceAs(1, 2, 4);
		}

		[Test]
		public async Task VerifyHistoryOfIng1Async()
		{
			var ed1_fromRev1 = new ListJoinColumnBidirectionalRefEdEntity {Id = ed1_id, Data = "ed1"};
			var ed1_fromRev3 = new ListJoinColumnBidirectionalRefEdEntity { Id = ed1_id, Data = "ed1 bis" };
			var ed2 = await (Session.GetAsync<ListJoinColumnBidirectionalRefEdEntity>(ed2_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<ListJoinColumnBidirectionalRefIngEntity>(ing1_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<ListJoinColumnBidirectionalRefIngEntity>(ing1_id, 2)).ConfigureAwait(false);
			var rev3 = await (AuditReader().FindAsync<ListJoinColumnBidirectionalRefIngEntity>(ing1_id, 3)).ConfigureAwait(false);
			var rev4 = await (AuditReader().FindAsync<ListJoinColumnBidirectionalRefIngEntity>(ing1_id, 4)).ConfigureAwait(false);

			rev1.References.Should().Have.SameValuesAs(ed1_fromRev1);
			rev2.References.Should().Have.SameValuesAs(ed1_fromRev1, ed2);
			rev3.References.Should().Have.SameValuesAs(ed1_fromRev3, ed2);
			rev4.References.Should().Be.Empty();
		}

		[Test]
		public async Task VerifyHistoryOfIng2Async()
		{
			var ed1 = await (Session.GetAsync<ListJoinColumnBidirectionalRefEdEntity>(ed1_id)).ConfigureAwait(false);
			var ed2 = await (Session.GetAsync<ListJoinColumnBidirectionalRefEdEntity>(ed2_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<ListJoinColumnBidirectionalRefIngEntity>(ing2_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<ListJoinColumnBidirectionalRefIngEntity>(ing2_id, 2)).ConfigureAwait(false);
			var rev3 = await (AuditReader().FindAsync<ListJoinColumnBidirectionalRefIngEntity>(ing2_id, 3)).ConfigureAwait(false);
			var rev4 = await (AuditReader().FindAsync<ListJoinColumnBidirectionalRefIngEntity>(ing2_id, 4)).ConfigureAwait(false);

			rev1.References.Should().Have.SameValuesAs(ed2);
			rev2.References.Should().Be.Empty();
			rev3.References.Should().Be.Empty();
			rev4.References.Should().Have.SameValuesAs(ed1, ed2);
		}

		[Test]
		public async Task VerifyHistoryOfEd1Async()
		{
			var ing1 = await (Session.GetAsync<ListJoinColumnBidirectionalRefIngEntity>(ing1_id)).ConfigureAwait(false);
			var ing2 = await (Session.GetAsync<ListJoinColumnBidirectionalRefIngEntity>(ing2_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<ListJoinColumnBidirectionalRefEdEntity>(ed1_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<ListJoinColumnBidirectionalRefEdEntity>(ed1_id, 2)).ConfigureAwait(false);
			var rev3 = await (AuditReader().FindAsync<ListJoinColumnBidirectionalRefEdEntity>(ed1_id, 3)).ConfigureAwait(false);
			var rev4 = await (AuditReader().FindAsync<ListJoinColumnBidirectionalRefEdEntity>(ed1_id, 4)).ConfigureAwait(false);

			Assert.AreEqual(ing1, rev1.Owner);
			Assert.AreEqual(ing1, rev2.Owner);
			Assert.AreEqual(ing1, rev3.Owner);
			Assert.AreEqual(ing2, rev4.Owner);

			Assert.AreEqual("ed1", rev1.Data);
			Assert.AreEqual("ed1", rev2.Data);
			Assert.AreEqual("ed1 bis", rev3.Data);
			Assert.AreEqual("ed1 bis", rev4.Data);
		}

		[Test]
		public async Task VerifyHistoryOfEd2Async()
		{
			var ing1 = await (Session.GetAsync<ListJoinColumnBidirectionalRefIngEntity>(ing1_id)).ConfigureAwait(false);
			var ing2 = await (Session.GetAsync<ListJoinColumnBidirectionalRefIngEntity>(ing2_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<ListJoinColumnBidirectionalRefEdEntity>(ed2_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<ListJoinColumnBidirectionalRefEdEntity>(ed2_id, 2)).ConfigureAwait(false);
			var rev3 = await (AuditReader().FindAsync<ListJoinColumnBidirectionalRefEdEntity>(ed2_id, 3)).ConfigureAwait(false);
			var rev4 = await (AuditReader().FindAsync<ListJoinColumnBidirectionalRefEdEntity>(ed2_id, 4)).ConfigureAwait(false);

			Assert.AreEqual(ing2, rev1.Owner);
			Assert.AreEqual(ing1, rev2.Owner);
			Assert.AreEqual(ing1, rev3.Owner);
			Assert.AreEqual(ing2, rev4.Owner);

			Assert.AreEqual("ed2", rev1.Data);
			Assert.AreEqual("ed2", rev2.Data);
			Assert.AreEqual("ed2", rev3.Data);
			Assert.AreEqual("ed2", rev4.Data);
		}
	}
}