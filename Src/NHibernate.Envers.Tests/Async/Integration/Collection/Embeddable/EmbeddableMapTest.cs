﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Collection;
using NHibernate.Envers.Tests.Entities.Components;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Collection.Embeddable
{
	using System.Threading.Tasks;
	public partial class EmbeddableMapTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountsAsync()
		{
			(await (AuditReader().GetRevisionsAsync(typeof(EmbeddableMapEntity), eme1Id)).ConfigureAwait(false))
									 .Should().Have.SameSequenceAs(1, 2, 3);
			(await (AuditReader().GetRevisionsAsync(typeof(EmbeddableMapEntity), eme2Id)).ConfigureAwait(false))
									 .Should().Have.SameSequenceAs(1, 3);
		}

		[Test]
		public async Task VerifyHistoryOfEme1Async()
		{
			var rev1 = await (AuditReader().FindAsync<EmbeddableMapEntity>(eme1Id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<EmbeddableMapEntity>(eme1Id, 2)).ConfigureAwait(false);
			var rev3 = await (AuditReader().FindAsync<EmbeddableMapEntity>(eme1Id, 3)).ConfigureAwait(false);
			var rev4 = await (AuditReader().FindAsync<EmbeddableMapEntity>(eme1Id, 4)).ConfigureAwait(false);

			rev1.ComponentMap.Should().Be.Empty();
			rev2.ComponentMap.Should().Have.SameValuesAs(new KeyValuePair<string, Component3>("1", c3_1), new KeyValuePair<string, Component3>("2", c3_2));
			rev3.ComponentMap.Should().Have.SameValuesAs(new KeyValuePair<string, Component3>("2", c3_2));
			rev4.ComponentMap.Should().Have.SameValuesAs(new KeyValuePair<string, Component3>("2", c3_2));
		}

		[Test]
		public async Task VerifyHistoryOfEme2Async()
		{
			var rev1 = await (AuditReader().FindAsync<EmbeddableMapEntity>(eme2Id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<EmbeddableMapEntity>(eme2Id, 2)).ConfigureAwait(false);
			var rev3 = await (AuditReader().FindAsync<EmbeddableMapEntity>(eme2Id, 3)).ConfigureAwait(false);
			var rev4 = await (AuditReader().FindAsync<EmbeddableMapEntity>(eme2Id, 4)).ConfigureAwait(false);


			rev1.ComponentMap.Should().Have.SameValuesAs(new KeyValuePair<string, Component3>("1", c3_1));
			rev2.ComponentMap.Should().Have.SameValuesAs(new KeyValuePair<string, Component3>("1", c3_1));
			rev3.ComponentMap.Should().Have.SameValuesAs(new KeyValuePair<string, Component3>("1", c3_2));
			rev4.ComponentMap.Should().Have.SameValuesAs(new KeyValuePair<string, Component3>("1", c3_2));
		}
	}
}