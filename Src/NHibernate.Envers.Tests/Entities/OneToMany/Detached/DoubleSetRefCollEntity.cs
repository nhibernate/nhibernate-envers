using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.OneToMany.Detached
{
	public class DoubleSetRefCollEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual string Data { get; set; }

		[Audited]
		public virtual ISet<StrTestEntity> Collection { get; set; }

		[Audited]
		public virtual ISet<StrTestEntity> Collection2 { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as SetRefCollEntity;
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