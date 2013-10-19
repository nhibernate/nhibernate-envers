using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Tests.Entities;

namespace NHibernate.Envers.Tests.Integration.Naming
{
	public class VersionsJoinTableTestEntity
	{
		public virtual int Id { get; set; }
		[Audited]
		public virtual string Data { get; set; }
		[Audited]
		[AuditJoinTable(TableName = "VERSIONS_JOIN_TABLE_TEST", InverseJoinColumns = new[]{"STR_ID"})]
		public virtual ISet<StrTestEntity> Collection { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as VersionsJoinTableTestEntity;
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