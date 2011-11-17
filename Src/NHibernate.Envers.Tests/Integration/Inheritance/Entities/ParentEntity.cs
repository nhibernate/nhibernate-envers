using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Entities
{
	[Audited]
	public class ParentEntity
	{
		public virtual int Id { get; set; }
		public virtual string Data { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ParentEntity;
			if (casted == null)
				return false;
			if (Id != casted.Id)
				return false;
			if (Data != null ? !Data.Equals(casted.Data) : casted.Data != null)
				return false;
			return true;
		}

		public override int GetHashCode()
		{
			var dataHash = (Data == null) ? 0 : Data.GetHashCode();
			return Id ^ dataHash;
		}
	}
}