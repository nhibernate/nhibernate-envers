using NUnit.Framework;

namespace NHibernate.Envers.Tests
{
	[TestFixture("NHibernate.Envers.Strategy.DefaultAuditStrategy, NHibernate.Envers")]
	[TestFixture("NHibernate.Envers.Strategy.ValidityAuditStrategy, NHibernate.Envers")]
	public abstract class TestBase : OneStrategyTestBase
	{
		protected TestBase(string strategyType) : base(strategyType)
		{
		}
	}
}