using System;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.BiDirOneToOneWithOneSideNotAudited
{
	[Audited]
	public class Parent
	{
		public virtual Guid Id { get; set; }

		[Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
		public virtual Child Child { get; set; }

		public virtual string Description { get; set; }
	}
}