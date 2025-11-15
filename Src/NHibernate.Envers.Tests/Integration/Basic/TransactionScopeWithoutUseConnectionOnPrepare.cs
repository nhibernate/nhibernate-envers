using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;
using System.Transactions;

namespace NHibernate.Envers.Tests.Integration.Basic
{
	public class TransactionScopeWithoutUseConnectionOnPrepare : TransactionScopeWithUseConnectionOnPrepare
	{
		public TransactionScopeWithoutUseConnectionOnPrepare(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

#if NETCOREAPP2_0
		[Ignore("System.Transactions is not supported in Core. See https://github.com/dotnet/corefx/issues/19894")]
#endif
		[Test]
		public void ShouldNotThrow()
		{
			var entity = new StrTestEntity { Str = "data" };

			Assert.DoesNotThrow(() =>
			{
				using (var distrTx = new TransactionScope())
				{
					using (var newSession = Session.SessionFactory.OpenSession())
					{
						newSession.FlushMode = FlushMode.Manual;
						newSession.Save(entity);
						newSession.Flush();
					}

					distrTx.Complete();
				}
			});
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{ 
			// It is recommended since NH 5
			configuration.SetProperty(NHibernate.Cfg.Environment.UseConnectionOnSystemTransactionPrepare, "false");

			base.AddToConfiguration(configuration);
		}
	}
}