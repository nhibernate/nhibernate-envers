﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToOne.BiDirectional
{
	using System.Threading.Tasks;
	public partial class BidirectionalNoNullsTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountAsync()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, await (AuditReader().GetRevisionsAsync(typeof(BiRefEdEntity), ed1_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, await (AuditReader().GetRevisionsAsync(typeof(BiRefEdEntity), ed2_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, await (AuditReader().GetRevisionsAsync(typeof(BiRefIngEntity), ing1_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, await (AuditReader().GetRevisionsAsync(typeof(BiRefIngEntity), ing2_id)).ConfigureAwait(false));
		}

		[Test]
		public async Task VerifyHistoryOfEd1Async()
		{
			var ing1 = await (Session.GetAsync<BiRefIngEntity>(ing1_id)).ConfigureAwait(false);
			var ing2 = await (Session.GetAsync<BiRefIngEntity>(ing2_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<BiRefEdEntity>(ed1_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<BiRefEdEntity>(ed1_id, 2)).ConfigureAwait(false);

			Assert.AreEqual(ing1, rev1.Referencing);
			Assert.AreEqual(ing2, rev2.Referencing);
		}

		[Test]
		public async Task VerifyHistoryOfEd2Async()
		{
			var ing1 = await (Session.GetAsync<BiRefIngEntity>(ing1_id)).ConfigureAwait(false);
			var ing2 = await (Session.GetAsync<BiRefIngEntity>(ing2_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<BiRefEdEntity>(ed2_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<BiRefEdEntity>(ed2_id, 2)).ConfigureAwait(false);

			Assert.AreEqual(ing2, rev1.Referencing);
			Assert.AreEqual(ing1, rev2.Referencing);
		}
	}
}