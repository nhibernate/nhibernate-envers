using NUnit.Framework;

namespace NHibernate.Envers.Tests
{
	[TestFixture]
	public abstract class ValidityTestBase : OneStrategyTestBase
	{
		protected ValidityTestBase()
			: base("NHibernate.Envers.Strategy.ValidityAuditStrategy, NHibernate.Envers")
		{
		}
	}
}