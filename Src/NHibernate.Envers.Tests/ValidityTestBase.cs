using NUnit.Framework;

namespace NHibernate.Envers.Tests
{
	[TestFixture]
	public abstract class ValidityTestBase : OneStrategyTestBase
	{
		protected ValidityTestBase()
			: base(AuditStrategyForTest.ValidityAuditStrategy)
		{
		}
	}
}