using System.Collections.Generic;
using NHibernate.Envers.Tests.Integration.ManyToMany;

namespace NHibernate.Envers.Tests.Entities.ManyToMany
{
	[Audited]
	public class WhereJoinTableEntity
	{
		public virtual int Id { get; set; }
		public virtual string Data { get; set; }
		public virtual IList<IntNoAutoIdTestEntity> References1 { get; set; }
		public virtual IList<IntNoAutoIdTestEntity> References2 { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as WhereJoinTableEntity;
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