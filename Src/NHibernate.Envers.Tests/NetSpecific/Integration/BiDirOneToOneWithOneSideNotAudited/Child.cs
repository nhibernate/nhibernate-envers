using System;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.BiDirOneToOneWithOneSideNotAudited
{
	public class Child
	{
		public virtual Guid Id { get; set; }
		public virtual Parent Parent { get; set; }
		public virtual string Str { get; set; }
	}
}