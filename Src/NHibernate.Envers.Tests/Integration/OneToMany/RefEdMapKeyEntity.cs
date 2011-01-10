using System.Collections.Generic;
using NHibernate.Envers.Compatibility.Attributes;

namespace NHibernate.Envers.Tests.Integration.OneToMany
{
	public class RefEdMapKeyEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		[MapKey(Name = "Data")]
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