﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Primitive
{
	using System.Threading.Tasks;
	public partial class PrimitiveAddDeleteTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountAsync()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, await (AuditReader().GetRevisionsAsync(typeof(PrimitiveTestEntity), id1)).ConfigureAwait(false));
		}

		[Test]
		public async Task VerifyHistoryOfId1Async()
		{
			var ver1 = new PrimitiveTestEntity { Id = id1, Number = 10, Number2 = 0 };
			var ver2 = new PrimitiveTestEntity { Id = id1, Number = 20, Number2 = 0 };

			Assert.AreEqual(ver1, await (AuditReader().FindAsync<PrimitiveTestEntity>(id1, 1)).ConfigureAwait(false));
			Assert.AreEqual(ver2, await (AuditReader().FindAsync<PrimitiveTestEntity>(id1, 2)).ConfigureAwait(false));
			Assert.IsNull(await (AuditReader().FindAsync<PrimitiveTestEntity>(id1, 3)).ConfigureAwait(false));
		}

		[Test]
		public async Task VerifyQueryWithDeletedAsync()
		{
			var entities = await (AuditReader().CreateQuery()
							.ForRevisionsOfEntity(typeof(PrimitiveTestEntity), true, true).GetResultListAsync()).ConfigureAwait(false);

			var expected = new[]
			               	{
			               		new PrimitiveTestEntity {Id = id1, Number = 10, Number2 = 0},
			               		new PrimitiveTestEntity {Id = id1, Number = 20, Number2 = 0},
			               		new PrimitiveTestEntity {Id = id1, Number = 0, Number2 = 0}
			               	};
			CollectionAssert.AreEqual(expected, entities);
		}

		[Test]
		public async Task VerifyQueryWithDeletedUsingGenericAsync()
		{
			var entities = await (AuditReader().CreateQuery().ForRevisionsOf<PrimitiveTestEntity>(true).ResultsAsync()).ConfigureAwait(false);

			var expected = new[]
			               	{
			               		new PrimitiveTestEntity {Id = id1, Number = 10, Number2 = 0},
			               		new PrimitiveTestEntity {Id = id1, Number = 20, Number2 = 0},
			               		new PrimitiveTestEntity {Id = id1, Number = 0, Number2 = 0}
			               	};
			entities.Should().Have.SameSequenceAs(expected);
		}

		[Test]
		public async Task VerifyQueryWithNoDeletedUsingGenericAsync()
		{
			var entities = await (AuditReader().CreateQuery()
							.ForRevisionsOf<PrimitiveTestEntity>().ResultsAsync()).ConfigureAwait(false);

			var expected = new[]
			               	{
			               		new PrimitiveTestEntity {Id = id1, Number = 10, Number2 = 0},
			               		new PrimitiveTestEntity {Id = id1, Number = 20, Number2 = 0}
			               	};
			entities.Should().Have.SameSequenceAs(expected);
		}
	}
}