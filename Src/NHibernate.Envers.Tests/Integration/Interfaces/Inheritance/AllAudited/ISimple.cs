using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Interfaces.Inheritance.AllAudited
{
	[Audited]
	public interface ISimple
	{
		long Id { get; set; }
		string Data { get; set; }
	}
}