using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.OneToMany.EmbeddedId
{
	[Audited]
	public class PersonTuple
	{
		public PersonTuple(bool helloWorld, Person personA, Person personB, Constant constant)
		{
			PersonTupleId = new PersonTupleId {PersonAId = personA.Id, PersonBId = personB.Id, ConstantId = constant.Id};

			HelloWorld = helloWorld;
			PersonA = personA;
			PersonB = personB;
			Constant = constant;

			personA.PersonATuples.Add(this);
			personB.PersonBTuples.Add(this);
		}

		protected PersonTuple(){}

		public virtual PersonTupleId PersonTupleId { get; protected set; }
		public virtual Person PersonA { get; set; }
		public virtual Person PersonB { get; set; }
		public virtual Constant Constant { get; set; }
		public virtual bool HelloWorld { get; set; }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (PersonTuple)) return false;
			return Equals((PersonTuple) obj);
		}

		public virtual bool Equals(PersonTuple other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.PersonTupleId, PersonTupleId);
		}

		public override int GetHashCode()
		{
			return (PersonTupleId != null ? PersonTupleId.GetHashCode() : 0);
		}
	}
}