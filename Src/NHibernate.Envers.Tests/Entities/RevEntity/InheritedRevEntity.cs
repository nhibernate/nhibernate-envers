using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.RevEntity
{
	[RevisionEntity]
	public class InheritedRevEntity : CustomRevEntity
	{
	}
}