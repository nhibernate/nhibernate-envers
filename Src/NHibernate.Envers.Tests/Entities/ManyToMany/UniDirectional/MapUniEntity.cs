using System.Collections.Generic;

namespace NHibernate.Envers.Tests.Entities.ManyToMany.UniDirectional
{
	public class MapUniEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual string Data { get; set; }

		[Audited]
		public virtual IDictionary<string, StrTestEntity> References { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as MapUniEntity;
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