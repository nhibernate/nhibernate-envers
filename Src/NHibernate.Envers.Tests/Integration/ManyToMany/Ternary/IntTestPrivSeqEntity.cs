using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.ManyToMany.Ternary
{
	public class IntTestPrivSeqEntity
	{
		public virtual int Id { get; set; }
		[Audited]
		public virtual int Number { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as IntTestPrivSeqEntity;
			if (casted == null)
				return false;
			return Id == 0 ? base.Equals(obj) : Id == casted.Id;
		}

		public override int GetHashCode()
		{
			return Id == 0 ? base.GetHashCode() : Id;
		}

		public override string ToString()
		{
			return "Id:" +Id + ", Number: " + Number;
		}
	}
}