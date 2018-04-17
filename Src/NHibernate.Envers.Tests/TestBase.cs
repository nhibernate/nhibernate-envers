using NUnit.Framework;

namespace NHibernate.Envers.Tests
{
	[TestFixture(AuditStrategyForTest.DefaultAuditStrategy)]
	[TestFixture(AuditStrategyForTest.ValidityAuditStrategy)]
	public abstract class TestBase : OneStrategyTestBase
	{
		protected TestBase(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}
	}
}