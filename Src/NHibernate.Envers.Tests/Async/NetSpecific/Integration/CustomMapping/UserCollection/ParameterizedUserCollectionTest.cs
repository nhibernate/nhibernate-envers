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

namespace NHibernate.Envers.Tests.NetSpecific.Integration.CustomMapping.UserCollection
{
	using System.Threading.Tasks;
	public partial class ParameterizedUserCollectionTest : TestBase
	{

		[Test]
		public async Task VerifyNhCoreMappingAsync()
		{
			var entity = await (Session.GetAsync<Entity>(id)).ConfigureAwait(false);
			entity.SpecialCollection.ItemsOverLimit()
				.Should().Be.EqualTo(3);
		}

		[Test]
		public async Task VerifyRevision1Async()
		{
			var rev1 = await (AuditReader().FindAsync<Entity>(id, 1)).ConfigureAwait(false);
			rev1.SpecialCollection.ItemsOverLimit()
				.Should().Be.EqualTo(1);
		}

		[Test]
		public async Task VerifyRevision2Async()
		{
			var rev1 = await (AuditReader().FindAsync<Entity>(id, 2)).ConfigureAwait(false);
			rev1.SpecialCollection.ItemsOverLimit()
				.Should().Be.EqualTo(3);
		}
	}
}