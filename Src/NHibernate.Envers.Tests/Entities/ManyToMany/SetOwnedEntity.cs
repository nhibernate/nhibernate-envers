using Iesi.Collections.Generic;

namespace NHibernate.Envers.Tests.Entities.ManyToMany
{
	public class SetOwnedEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual string Data { get; set; }

		[Audited]
		public virtual ISet<SetOwningEntity> Referencing { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as SetOwnedEntity;
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