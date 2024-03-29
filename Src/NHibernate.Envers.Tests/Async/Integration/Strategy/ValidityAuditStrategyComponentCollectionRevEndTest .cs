﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Tests.Integration.Strategy.Model;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Strategy
{
	using System.Threading.Tasks;
	public partial class ValidityAuditStrategyComponentCollectionRevEndTest : ValidityTestBase
	{

		[Test]
		public async Task VerifyRevisionCountsAsync()
		{
			(await (AuditReader().GetRevisionsAsync(typeof(Product), productId)).ConfigureAwait(false))
				.Should().Have.SameSequenceAs(1, 2, 3);
		}

		[Test]
		public async Task VerifyRevision1Async()
		{
			var product = await (AuditReader().FindAsync<Product>(productId, 1)).ConfigureAwait(false);
			product.Items.Single().Name.Should().Be.EqualTo("bread");
		}

		[Test]
		public async Task VerifyRevision2Async()
		{
			var product = await (AuditReader().FindAsync<Product>(productId, 2)).ConfigureAwait(false);
			product.Items.Select(x => x.Name)
				.Should().Have.SameSequenceAs("bread", "bread2");
		}

		[Test]
		public async Task VerifyRevision3Async()
		{
			var product = await (AuditReader().FindAsync<Product>(productId, 3)).ConfigureAwait(false);
			product.Items.Single().Name.Should().Be.EqualTo("bread2");
		}
	}
}