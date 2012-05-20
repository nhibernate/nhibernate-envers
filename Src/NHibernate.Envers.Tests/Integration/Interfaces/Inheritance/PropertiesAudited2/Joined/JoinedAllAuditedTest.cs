using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Interfaces.Inheritance.PropertiesAudited2.Joined
{
	[TestFixture]
	public class JoinedAllAuditedTest : AbstractAllAuditedTest
	{
		public JoinedAllAuditedTest(string strategyType) : base(strategyType)
		{
		}
	}
}