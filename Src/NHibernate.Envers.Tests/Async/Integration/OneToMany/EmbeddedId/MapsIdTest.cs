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

namespace NHibernate.Envers.Tests.Integration.OneToMany.EmbeddedId
{
	using System.Threading.Tasks;
	public partial class MapsIdTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountAsync()
		{
			(await (AuditReader().GetRevisionsAsync(typeof (PersonTuple), tuple1Ver1.PersonTupleId)).ConfigureAwait(false)).Should().Have.SameSequenceAs(1);
			(await (AuditReader().GetRevisionsAsync(typeof (PersonTuple), tuple2Ver1.PersonTupleId)).ConfigureAwait(false)).Should().Have.SameSequenceAs(2, 3);
			(await (AuditReader().GetRevisionsAsync(typeof (Person), personCVer1.Id)).ConfigureAwait(false)).Should().Have.SameSequenceAs(2, 4);
		}

		[Test]
		public async Task VerifyHistoryOfTuple1Async()
		{
			var tuple = await (AuditReader().FindAsync<PersonTuple>(tuple1Ver1.PersonTupleId, 1)).ConfigureAwait(false);
			tuple.Should().Be.EqualTo(tuple1Ver1);
			tuple.HelloWorld.Should().Be.EqualTo(tuple1Ver1.HelloWorld);
			tuple.PersonA.Id.Should().Be.EqualTo(tuple1Ver1.PersonA.Id);
			tuple.PersonB.Id.Should().Be.EqualTo(tuple1Ver1.PersonB.Id);
		}

		[Test]
		public async Task VerifyHistoryOfTuple2Async()
		{
			var tuple = await (AuditReader().FindAsync<PersonTuple>(tuple2Ver2.PersonTupleId, 2)).ConfigureAwait(false);
			tuple.Should().Be.EqualTo(tuple2Ver1);
			tuple.HelloWorld.Should().Be.EqualTo(tuple2Ver1.HelloWorld);
			tuple.PersonA.Id.Should().Be.EqualTo(tuple2Ver1.PersonA.Id);
			tuple.PersonB.Id.Should().Be.EqualTo(tuple2Ver1.PersonB.Id);

			tuple = await (AuditReader().FindAsync<PersonTuple>(tuple2Ver2.PersonTupleId, 3)).ConfigureAwait(false);
			tuple.Should().Be.EqualTo(tuple2Ver2);
			tuple.HelloWorld.Should().Be.EqualTo(tuple2Ver2.HelloWorld);
			tuple.PersonA.Id.Should().Be.EqualTo(tuple2Ver2.PersonA.Id);
			tuple.PersonB.Id.Should().Be.EqualTo(tuple2Ver2.PersonB.Id);
		}

		[Test]
		public async Task VerifyHistoryOfPersonCAsync()
		{
			var person = await (AuditReader().FindAsync<Person>(personCVer1.Id, 2)).ConfigureAwait(false);
			person.Should().Be.EqualTo(personCVer1);
			person.PersonATuples.Should().Have.SameValuesAs(personCVer1.PersonATuples);
			person.PersonBTuples.Should().Have.SameValuesAs(personCVer1.PersonBTuples);

			person = await (AuditReader().FindAsync<Person>(personCVer2.Id, 4)).ConfigureAwait(false);
			person.Should().Be.EqualTo(personCVer2);
		}
	}
}