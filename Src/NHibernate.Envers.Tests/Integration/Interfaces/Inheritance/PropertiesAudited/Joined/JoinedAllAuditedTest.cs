using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Interfaces.Inheritance.PropertiesAudited.Joined
{
	[TestFixture]
	public class JoinedAllAuditedTest : AbstractAllAuditedTest
	{
		public JoinedAllAuditedTest(string strategyType) : base(strategyType)
		{
		}
	}
}