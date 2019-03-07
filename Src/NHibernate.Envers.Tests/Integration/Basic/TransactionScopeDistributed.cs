using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Basic
{
	public class TransactionScopeDistributed : TestBase
	{
		public TransactionScopeDistributed(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

#if NETCOREAPP2_0
		[Ignore("Distributed transaction is not supported in Core.")]
#endif
	   [Test]
		public void ShouldNotThrowWhenDistributedTransaction()
		{
			var entity = new StrTestEntity { Str = "data" };

			Assert.DoesNotThrow(() =>
			{
				using (var distrTx = new TransactionScope())
				{
					// Simulation of durable resource enlistment
					using (var newSession = Session.SessionFactory.OpenSession())
					{
						newSession.FlushMode = FlushMode.Manual;
						newSession.Query<StrTestEntity>().FirstOrDefault();
					}

					using (var newSession2 = Session.SessionFactory.OpenSession())
					{
						newSession2.FlushMode = FlushMode.Manual;
						newSession2.Save(entity);
						newSession2.Flush();
					}

					distrTx.Complete();
				}
			});
		}

#if NETCOREAPP2_0
		[Ignore("Distributed transaction is not supported in Core.")]
#endif
		[Test]
		public void ShouldNotThrowWhenDistributedTransactionWithAutoFlushMode()
		{
			var entity = new StrTestEntity { Str = "data" };

			Assert.DoesNotThrow(() =>
			{
				using (var distrTx = new TransactionScope())
				{
					using (var newSession = Session.SessionFactory.OpenSession())
					{
						newSession.FlushMode = FlushMode.Auto;
						newSession.Query<StrTestEntity>().FirstOrDefault();
					}

					using (var newSession2 = Session.SessionFactory.OpenSession())
					{
						newSession2.FlushMode = FlushMode.Auto;
						newSession2.Save(entity);
						newSession2.Flush();
					}

					distrTx.Complete();
				}
			});
		}

#if NETCOREAPP2_0
		[Ignore("Distributed transaction is not supported in Core.")]
#endif
		[Test]
		public void ShouldNotThrowWhenDistributedTransactionWithNhTransaction()
		{
			var entity = new StrTestEntity { Str = "data" };

			Assert.DoesNotThrow(() =>
			{
				using (var distrTx = new TransactionScope())
				{
					using (var newSession = Session.SessionFactory.OpenSession())
					{
						newSession.Query<StrTestEntity>().FirstOrDefault();
					}

					using (var newSession2 = Session.SessionFactory.OpenSession())
					{
						using (var nhTx = newSession2.BeginTransaction())
						{
							newSession2.Save(entity);
							newSession2.Flush();
							nhTx.Commit();
						}
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
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] {"Entities.Mapping.hbm.xml", "Integration.Collection.NoRevision.Mapping.hbm.xml"}; }
		}
	}
}