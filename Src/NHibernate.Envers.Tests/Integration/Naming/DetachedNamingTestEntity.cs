using Iesi.Collections.Generic;
using NHibernate.Envers.Tests.Entities;

namespace NHibernate.Envers.Tests.Integration.Naming
{
	public class DetachedNamingTestEntity
	{
		public virtual int Id { get; set; }
		[Audited]
		public virtual string Data { get; set; }
		[Audited]
		public virtual ISet<StrTestEntity> Collection { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as DetachedNamingTestEntity;
			if (casted == null)
				return false;
			return (Id == casted.Id && Data.Equals(casted.Data));
		}

		public override int GetHashCode()
		{
			return Id ^ Data.GetHashCode();
		}
	}
}