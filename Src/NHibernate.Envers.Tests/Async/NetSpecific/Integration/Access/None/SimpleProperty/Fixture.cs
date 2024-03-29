﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using NHibernate.Envers.Query;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Access.None.SimpleProperty
{
	using System.Threading.Tasks;
	public partial class Fixture : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountsAsync()
		{
			CollectionAssert.AreEqual(new[] { 1, 2 }, await (AuditReader().GetRevisionsAsync(typeof(Entity), id)).ConfigureAwait(false));
		}

		[Test]
		public async Task VerifyHistoryOfComponentAsync()
		{
			var ver1 = await (AuditReader().FindAsync<Entity>(id, 1)).ConfigureAwait(false);
			var ver2 = await (AuditReader().FindAsync<Entity>(id, 2)).ConfigureAwait(false);

			ver1.Data.Should().Be.EqualTo(1);

			ver2.Data.Should().Be.EqualTo(2);
		}

		[Test]
		public async Task CanQueryOnAccessNoneDataAsync()
		{
			var res = (IList)await (AuditReader().CreateQuery()
				.ForRevisionsOfEntity(typeof (Entity), false, false)
				.Add(AuditEntity.Property("Data2").Eq(2))
				.GetSingleResultAsync()).ConfigureAwait(false);

			var ent = (Entity) res[0];
			var rev = (DefaultRevisionEntity) res[1];

			rev.Id.Should().Be.EqualTo(2);
			ent.Data.Should().Be.EqualTo(2);
		}

		[Test]
		public async Task CanQueryOnAccessNoopDataAsync()
		{
			var res = (IList)await (AuditReader().CreateQuery()
				.ForRevisionsOfEntity(typeof(Entity), false, false)
				.Add(AuditEntity.Property("Data3").Eq(2))
				.GetSingleResultAsync()).ConfigureAwait(false);

			var ent = (Entity)res[0];
			var rev = (DefaultRevisionEntity)res[1];

			rev.Id.Should().Be.EqualTo(2);
			ent.Data.Should().Be.EqualTo(2);
		}
	}
}