using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Entities
{
	[Audited]
	public class EmptyChildEntity : ParentEntity
	{
	}
}