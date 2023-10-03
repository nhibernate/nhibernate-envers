using System.Collections.Generic;
using System.Transactions;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Integration.Collection.NoRevision;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Basic
{
	public partial class OutsideTransactionTest : TestBase
	{
		public OutsideTransactionTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		[Test]
		public void ShouldThrowIfInsertOutsideActiveTransaction()
		{
			// Illegal insertion of entity outside of active transaction.
			var entity = new StrTestEntity {Str = "data"};
			Assert.Throws<AuditException>(() =>
			                              	{
															Session.Save(entity);
			                              		Session.Flush();
			                              	});
		}

		[Test]
		public void ShouldThrowIfUpdateOutsideActiveTransaction()
		{
			var entity = new StrTestEntity { Str = "data" };
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(entity);
				tx.Commit();
			}
			// Illegal modification of entity state outside of active transaction.
			entity.Str = "modified data";
			Assert.Throws<AuditException>(() => {
																Session.Update(entity);
																Session.Flush();
															});
		}

		[Test]
		public void ShouldThrowIfDeleteOutsideActiveTransaction()
		{
			var entity = new StrTestEntity { Str = "data" };
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(entity);
				tx.Commit();
			}
			// Illegal modification of entity state outside of active transaction.
			Assert.Throws<AuditException>(() =>
			                              	{
			                              		Session.Delete(entity);
			                              		Session.Flush();
			                              	});
		}

		[Test]
		public void ShouldThrowIfCollectionUpdateOutsideActiveTransaction()
		{
			var person = new Person();
			var name = new Name {TheName = "Name"};
			person.Names.Add(name);
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(person);
				tx.Commit();
			}
			// Illegal collection update outside of active transaction.
			person.Names.Remove(name);
			Assert.Throws<AuditException>(() =>
			                              	{
			                              		Session.Update(person);
			                              		Session.Flush();
			                              	});
		}

		[Test]
		public void ShouldThrowIfCollectionRemovalOutsideActiveTransaction()
		{
			var person = new Person();
			var name = new Name {TheName = "Name"};
			person.Names.Add(name);
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(person);
				tx.Commit();
			}
			// Illegal collection update outside of active transaction.
			person.Names = null;
			Assert.Throws<AuditException>(() =>
			                              	{
			                              		Session.Update(person);
			                              		Session.Flush();
			                              	});
		}

#if NETCOREAPP2_0_OR_GREATER
		[Ignore("System.Transactions is not supported in Core. See https://github.com/dotnet/corefx/issues/19894")]
#endif	
		[Test]
		public void ShouldThrowIfOutsideDistributedTransaction()
		{
			var entity = new StrTestEntity { Str = "data" };
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

			// Illegal insertion of entity outside of active transaction.
			Assert.Throws<AuditException>(() =>
			{
				Session.Save(entity);
				Session.Flush();
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