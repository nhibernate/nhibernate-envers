using System.Collections.Generic;

namespace NHibernate.Envers.Tests.Integration.Naming
{
	public class JoinNamingRefEdEntity
	{
		public virtual int Id { get; set; }
		[Audited]
		public virtual string Data { get; set; }
		[Audited]
		public virtual IList<JoinNamingRefIngEntity> Reffering { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as JoinNamingRefEdEntity;
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