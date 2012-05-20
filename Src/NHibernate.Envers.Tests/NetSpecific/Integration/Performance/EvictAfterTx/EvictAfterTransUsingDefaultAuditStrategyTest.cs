using NHibernate.Envers.Strategy;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Performance.EvictAfterTx
{
	public class EvictAfterTransUsingDefaultAuditStrategyTest : EvictAuditDataAfterCommitTest
	{
		public EvictAfterTransUsingDefaultAuditStrategyTest(string strategyType) : base(strategyType)
		{
		}

		protected override System.Type AuditStrategy
		{
			get { return typeof(DefaultAuditStrategy); }
		}
	}
}