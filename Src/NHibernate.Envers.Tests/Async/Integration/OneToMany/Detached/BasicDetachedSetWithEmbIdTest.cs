﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Ids;
using NHibernate.Envers.Tests.Entities.OneToMany.Detached.Ids;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToMany.Detached
{
	using System.Threading.Tasks;
	public partial class BasicDetachedSetWithEmbIdTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountAsync()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4 }, await (AuditReader().GetRevisionsAsync(typeof(SetRefCollEntityEmbId), coll1_id)).ConfigureAwait(false));

			CollectionAssert.AreEquivalent(new[] { 1 }, await (AuditReader().GetRevisionsAsync(typeof(EmbIdTestEntity), str1_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 1 }, await (AuditReader().GetRevisionsAsync(typeof(EmbIdTestEntity), str2_id)).ConfigureAwait(false));
		}

		[Test]
		public async Task VerifyHistoryOfColl1Async()
		{
			var str1 = await (Session.GetAsync<EmbIdTestEntity>(str1_id)).ConfigureAwait(false);
			var str2 = await (Session.GetAsync<EmbIdTestEntity>(str2_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<SetRefCollEntityEmbId>(coll1_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<SetRefCollEntityEmbId>(coll1_id, 2)).ConfigureAwait(false);
			var rev3 = await (AuditReader().FindAsync<SetRefCollEntityEmbId>(coll1_id, 3)).ConfigureAwait(false);
			var rev4 = await (AuditReader().FindAsync<SetRefCollEntityEmbId>(coll1_id, 4)).ConfigureAwait(false);

			CollectionAssert.AreEquivalent(new[] { str1 }, rev1.Collection);
			CollectionAssert.AreEquivalent(new[] { str1, str2 }, rev2.Collection);
			CollectionAssert.AreEquivalent(new[] { str2 }, rev3.Collection);
			CollectionAssert.IsEmpty(rev4.Collection);

			Assert.AreEqual("coll1", rev1.Data);
			Assert.AreEqual("coll1", rev2.Data);
			Assert.AreEqual("coll1", rev3.Data);
			Assert.AreEqual("coll1", rev4.Data);
		}
	}
}