using NHibernate.Envers.Strategy;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Performance.EvictAfterTx
{
	[TestFixture]
	[Ignore("Need a fix in NH Core first - https://nhibernate.jira.com/browse/NH-2907")]
	public class EvictAfterTransUsingValidityAuditStrategyTest : EvictAuditDataAfterCommitTest
	{
		protected override System.Type AuditStrategy
		{
			get { return typeof(ValidityAuditStrategy); }
		}
	}
}