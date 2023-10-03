using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.OneToMany.EmbeddedId
{
	public partial class MapsIdTest : TestBase
	{
		private PersonTuple tuple1Ver1;
		private PersonTuple tuple2Ver1;
		private PersonTuple tuple2Ver2;
		private Person personCVer1;
		private Person personCVer2;

		public MapsIdTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var personA = new Person { Name = "Peter" };
			var personB = new Person { Name = "Mary" };
			var cons = new Constant { Id = "USD", Name = "US Dollars" };

			//Revision 1
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(personA);
				Session.Save(personB);
				Session.Save(cons);
				var tuple = new PersonTuple(true, personA, personB, cons);
				Session.Save(tuple);
				tx.Commit();
				tuple1Ver1 = createTuple(tuple);
			}

			//Revision 2
			var personC1 = new Person { Name = "Lukasz" };
			PersonTuple tuple2;
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(personC1);
				tuple2 = new PersonTuple(true, personA, personC1, cons);
				Session.Save(tuple2);
				tx.Commit();
				tuple2Ver1 = createTuple(tuple2);
				personCVer1 = new Person {Id = personC1.Id, Name = personC1.Name};
				personCVer1.PersonBTuples.Add(tuple2Ver1);
			}

			//Revision 3
			using (var tx = Session.BeginTransaction())
			{
				tuple2.HelloWorld = false;
				tx.Commit();
				tuple2Ver2 = createTuple(tuple2);
			}

			//Revision 4
			using (var tx = Session.BeginTransaction())
			{
				personC1.Name = "Robert";
				tx.Commit();
				personCVer2 = new Person {Id = personC1.Id, Name = personC1.Name};
				personCVer2.PersonBTuples.Add(tuple2Ver1);
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			AuditReader().GetRevisions(typeof (PersonTuple), tuple1Ver1.PersonTupleId).Should().Have.SameSequenceAs(1);
			AuditReader().GetRevisions(typeof (PersonTuple), tuple2Ver1.PersonTupleId).Should().Have.SameSequenceAs(2, 3);
			AuditReader().GetRevisions(typeof (Person), personCVer1.Id).Should().Have.SameSequenceAs(2, 4);
		}

		[Test]
		public void VerifyHistoryOfTuple1()
		{
			var tuple = AuditReader().Find<PersonTuple>(tuple1Ver1.PersonTupleId, 1);
			tuple.Should().Be.EqualTo(tuple1Ver1);
			tuple.HelloWorld.Should().Be.EqualTo(tuple1Ver1.HelloWorld);
			tuple.PersonA.Id.Should().Be.EqualTo(tuple1Ver1.PersonA.Id);
			tuple.PersonB.Id.Should().Be.EqualTo(tuple1Ver1.PersonB.Id);
		}

		[Test]
		public void VerifyHistoryOfTuple2()
		{
			var tuple = AuditReader().Find<PersonTuple>(tuple2Ver2.PersonTupleId, 2);
			tuple.Should().Be.EqualTo(tuple2Ver1);
			tuple.HelloWorld.Should().Be.EqualTo(tuple2Ver1.HelloWorld);
			tuple.PersonA.Id.Should().Be.EqualTo(tuple2Ver1.PersonA.Id);
			tuple.PersonB.Id.Should().Be.EqualTo(tuple2Ver1.PersonB.Id);

			tuple = AuditReader().Find<PersonTuple>(tuple2Ver2.PersonTupleId, 3);
			tuple.Should().Be.EqualTo(tuple2Ver2);
			tuple.HelloWorld.Should().Be.EqualTo(tuple2Ver2.HelloWorld);
			tuple.PersonA.Id.Should().Be.EqualTo(tuple2Ver2.PersonA.Id);
			tuple.PersonB.Id.Should().Be.EqualTo(tuple2Ver2.PersonB.Id);
		}

		[Test]
		public void VerifyHistoryOfPersonC()
		{
			var person = AuditReader().Find<Person>(personCVer1.Id, 2);
			person.Should().Be.EqualTo(personCVer1);
			person.PersonATuples.Should().Have.SameValuesAs(personCVer1.PersonATuples);
			person.PersonBTuples.Should().Have.SameValuesAs(personCVer1.PersonBTuples);

			person = AuditReader().Find<Person>(personCVer2.Id, 4);
			person.Should().Be.EqualTo(personCVer2);
		}

		private static PersonTuple createTuple(PersonTuple tuple)
		{
			return new PersonTuple(tuple.HelloWorld, tuple.PersonA, tuple.PersonB, tuple.Constant);
		}
	}
}