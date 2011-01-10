using System.Collections.Generic;

namespace NHibernate.Envers.Tests.Entities.OneToMany.Detached
{
	public class ListRefCollEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual string Data { get; set; }

		[Audited]
		public virtual IList<StrTestEntity> Collection { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ListRefCollEntity;
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