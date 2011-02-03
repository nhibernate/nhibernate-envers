using System.Collections.Generic;

namespace NHibernate.Envers.Tests.Integration.OneToMany
{
	public class RefEdMapKeyEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual IDictionary<string, RefIngMapKeyEntity> IdMap { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as RefEdMapKeyEntity;
			if (casted == null)
				return false;
			return Id == casted.Id;
		}

		public override int GetHashCode()
		{
			return Id;
		}
	}
}