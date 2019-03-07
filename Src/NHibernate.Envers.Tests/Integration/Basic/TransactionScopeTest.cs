using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Basic
{
	public class TransactionScopeTest : TestBase
	{
		public TransactionScopeTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

#if NETCOREAPP2_0
		[Ignore("System.Transactions is not supported in Core. See https://github.com/dotnet/corefx/issues/19894")]
#endif
		[Test]
		public void ShouldNotThrowIfUseConnectionOnSystemTransactionPrepareIsFalse()
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

		protected override void Initialize()
		{
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			ConfigurationKey.StoreDataAtDelete.SetUserValue(configuration, true);

			// It is recommended since NH 5
			configuration.SetProperty(NHibernate.Cfg.Environment.UseConnectionOnSystemTransactionPrepare, "false");
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] {"Entities.Mapping.hbm.xml", "Integration.Collection.NoRevision.Mapping.hbm.xml"}; }
		}
	}
}