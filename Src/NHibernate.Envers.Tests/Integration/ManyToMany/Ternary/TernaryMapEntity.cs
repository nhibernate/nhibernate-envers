using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;

namespace NHibernate.Envers.Tests.Integration.ManyToMany.Ternary
{
	public class TernaryMapEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual IDictionary<IntTestEntity, StrTestEntity> Map { get; set; }

		public TernaryMapEntity()
		{
			Map = new Dictionary<IntTestEntity, StrTestEntity>();
		}

		public override bool Equals(object obj)
		{
			var casted = obj as TernaryMapEntity;
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