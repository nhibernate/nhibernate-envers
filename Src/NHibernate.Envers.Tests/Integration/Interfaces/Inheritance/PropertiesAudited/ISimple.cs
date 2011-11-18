using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Interfaces.Inheritance.PropertiesAudited
{
	public interface ISimple
	{
		long Id { get; set; }
		string Data { get; set; }
		[Audited]
		int Number { get; set; }
	}
}