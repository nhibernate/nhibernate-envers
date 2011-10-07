using NHibernate.Envers.Strategy;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Performance.EvictAfterTx
{
	[TestFixture]
	public class EvictAfterTransUsingDefaultAuditStrategyTest : EvictAuditDataAfterCommitTest
	{
		protected override System.Type AuditStrategy
		{
			get { return typeof(DefaultAuditStrategy); }
		}
	}
}