using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Entities
{
	[Audited]
	public class ParentIngEntity
	{
		public virtual int Id { get; set; }
		public virtual string Data { get; set; }
		public virtual ReferencedToParentEntity Referenced { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ParentIngEntity;
			if (casted == null)
				return false;
			return (Id == casted.Id && Data == casted.Data);
		}

		public override int GetHashCode()
		{
			return Id ^ Data.GetHashCode();
		}
	}
}