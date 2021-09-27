using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.BidirectionalSameColumn
{
	public class Model
	{
		public virtual int Id { get; set; }

		[Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
		[AuditJoinTable(TableName = "jointable")] //needed due to firebird limitation in table name length
		public virtual ISet<ModelConfigurationShared> ModelConfigurations { get; set; }
	}
}
