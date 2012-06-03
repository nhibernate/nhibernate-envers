using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.ManyToMany.Ternary
{
	public class TernaryMapEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual IDictionary<IntTestPrivSeqEntity, StrTestPrivSeqEntity> Map { get; set; }

		public TernaryMapEntity()
		{
			Map = new Dictionary<IntTestPrivSeqEntity, StrTestPrivSeqEntity>();
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