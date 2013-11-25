using System;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.NotFoundIgnore.BaseType
{
	[Audited]
	public abstract class Child
	{
		public virtual Guid Id { get; set; }
		[Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
		public virtual ChildName Name { get; set; }
		public abstract string Sex { get; }
	}

	[Audited]
	public class Boy :Child
	{
		public override string Sex
		{
			get { return "Boy"; }
		}
	}

	[Audited]
	public class Girl:Child
	{
		public override string Sex
		{
			get { return "Girl"; }
		}
	}
}