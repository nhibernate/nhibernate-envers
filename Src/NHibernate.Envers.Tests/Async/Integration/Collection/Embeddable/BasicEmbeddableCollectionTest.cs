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

namespace NHibernate.Envers.Tests.Integration.Collection.Embeddable
{
	using System.Threading.Tasks;
	public partial class BasicEmbeddableCollectionTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountsAsync()
		{
			(await (AuditReader().GetRevisionsAsync(typeof (DarkCharacter), id)).ConfigureAwait(false))
			             .Should().Have.SameSequenceAs(1, 2, 3, 4, 5);
		}

		[Test]
		public async Task VerifyHistoryOfCharacterAsync()
		{
			var darkCharacter = new DarkCharacter {Id = id, Kills = 1};
			var ver1 = await (AuditReader().FindAsync<DarkCharacter>(id, 1)).ConfigureAwait(false);
			ver1.Should().Be.EqualTo(darkCharacter);
			ver1.Names.Should().Be.Empty();

			darkCharacter.Names.Add(new Name { FirstName = "Action", LastName = "Hank" });
			var ver2 = await (AuditReader().FindAsync<DarkCharacter>(id, 2)).ConfigureAwait(false);
			ver2.Should().Be.EqualTo(darkCharacter);
			ver2.Names.Should().Have.SameValuesAs(darkCharacter.Names);

			darkCharacter.Names.Add(new Name { FirstName = "Green", LastName = "Lantern" });
			var ver3 = await (AuditReader().FindAsync<DarkCharacter>(id, 3)).ConfigureAwait(false);
			ver3.Should().Be.EqualTo(darkCharacter);
			ver3.Names.Should().Have.SameValuesAs(darkCharacter.Names);

			darkCharacter.Names.Remove(new Name { FirstName = "Action", LastName = "Hank" });
			var ver4 = await (AuditReader().FindAsync<DarkCharacter>(id, 4)).ConfigureAwait(false);
			ver4.Should().Be.EqualTo(darkCharacter);
			ver4.Names.Should().Have.SameValuesAs(darkCharacter.Names);

			darkCharacter.Names.Clear();
			var ver5 = await (AuditReader().FindAsync<DarkCharacter>(id, 5)).ConfigureAwait(false);
			ver5.Should().Be.EqualTo(darkCharacter);
			ver5.Names.Should().Have.SameValuesAs(darkCharacter.Names);
		}
	}
}