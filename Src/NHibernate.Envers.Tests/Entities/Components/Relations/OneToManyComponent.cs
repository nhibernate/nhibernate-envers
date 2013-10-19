using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.Components.Relations
{
	public class OneToManyComponent
	{
		public OneToManyComponent()
		{
			Entities = new HashSet<StrTestEntity>();
		}

		[AuditJoinTable(TableName = "OTMComp_StrTestEnt")]
		public virtual ISet<StrTestEntity> Entities { get; set; }
		public virtual string Data { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as OneToManyComponent;
			if (other == null)
				return false;
			if (!Data.Equals(other.Data))
				return false;
			if (Entities.Count != other.Entities.Count)
				return false;
			return Entities.All(strTestEntity => other.Entities.Contains(strTestEntity));
		}

		public override int GetHashCode()
		{
			return Data.GetHashCode() ^ Entities.GetHashCode();
		}
	}
}