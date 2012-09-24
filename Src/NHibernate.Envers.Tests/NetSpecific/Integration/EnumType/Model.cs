using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.EnumType
{
	[Audited]
	public class Entity
	{
		public virtual int Id { get; set; }
		public virtual EntityEnum EntityEnum { get; set; }
	}

	public enum EntityEnum
	{
		TypeA = 'A',
		TypeB = 'B'
	}
}