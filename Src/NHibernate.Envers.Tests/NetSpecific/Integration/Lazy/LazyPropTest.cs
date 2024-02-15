using NHibernate.Envers.Query;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Lazy
{
	public class LazyPropTest : TestBase
	{
		public LazyPropTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
		}

		[Test]
		public void UpdateLazyPropInSameSession()
		{
			string notLazyStart = "notLazyNotUpd",
				lazyStart = "lazyBefore",
				notLazyEnd = "notLazyNotUpd",
				lazyEnd = "lazyAfrer";
			
			var factory = Cfg.BuildSessionFactory();

			using (var s = factory.OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.SaveOrUpdate(new EntityWithLazyProp
					{
						Id = 1,
						NotLazyProp = notLazyStart,
						LazyProp = lazyStart
					});
					tx.Commit();
				}

				using (var tx = s.BeginTransaction())
				{
					var ent = s.Get<EntityWithLazyProp>(1);

					ent.LazyProp = lazyEnd;

					s.SaveOrUpdate(ent);
					tx.Commit();
				}
			}

			var current = factory.OpenSession().Get<EntityWithLazyProp>(1);

			Assert.AreEqual(notLazyEnd, current.NotLazyProp);
			Assert.AreEqual(lazyEnd, current.LazyProp);

			var history = AuditReader()
				.CreateQuery()
				.ForRevisionsOfEntity(typeof(EntityWithLazyProp), false, true)
				.Add(AuditEntity.Id().Eq(1))
				.GetResultList();

			Assert.AreEqual(2, history.Count);
		}

		[Test]
		public void UpdateNotLazyPropInSameSession()
		{
			string notLazyStart = "notLazyBefore",
				lazyStart = "lazyNotUpd",
				notLazyEnd = "notLazyAfrer",
				lazyEnd = "lazyNotUpd";

			var factory = Cfg.BuildSessionFactory();

			using (var s = factory.OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.SaveOrUpdate(new EntityWithLazyProp
					{
						Id = 1,
						NotLazyProp = notLazyStart,
						LazyProp = lazyStart
					});
					tx.Commit();
				}

				using (var tx = s.BeginTransaction())
				{
					var ent = s.Get<EntityWithLazyProp>(1);

					ent.NotLazyProp = notLazyEnd;

					s.SaveOrUpdate(ent);
					tx.Commit();
				}
			}

			var current = factory.OpenSession().Get<EntityWithLazyProp>(1);

			Assert.AreEqual(notLazyEnd, current.NotLazyProp);
			Assert.AreEqual(lazyEnd, current.LazyProp);

			var history = AuditReader()
				.CreateQuery()
				.ForRevisionsOfEntity(typeof(EntityWithLazyProp), false, true)
				.Add(AuditEntity.Id().Eq(1))
				.GetResultList();

			Assert.AreEqual(2, history.Count);
		}

		[Test]
		public void UpdateLazyPropInOtherSession()
		{
			string notLazyStart = "notLazyNotUpd",
				lazyStart = "lazyBefore",
				notLazyEnd = "notLazyNotUpd",
				lazyEnd = "lazyAfrer";

			var factory = Cfg.BuildSessionFactory();

			using (var s = factory.OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.SaveOrUpdate(new EntityWithLazyProp
					{
						Id = 1,
						NotLazyProp = notLazyStart,
						LazyProp = lazyStart
					});
					tx.Commit();
				}
			}

			using (var s = factory.OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					EntityWithLazyProp ent = s.Get<EntityWithLazyProp>(1);

					ent.LazyProp = lazyEnd;

					s.SaveOrUpdate(ent);
					tx.Commit();
				}
			}

			var current = Session.Get<EntityWithLazyProp>(1);

			Assert.AreEqual(notLazyEnd, current.NotLazyProp);
			Assert.AreEqual(lazyEnd, current.LazyProp);

			var history = AuditReader()
				.CreateQuery()
				.ForRevisionsOfEntity(typeof(EntityWithLazyProp), false, true)
				.Add(AuditEntity.Id().Eq(1))
				.GetResultList();

			Assert.AreEqual(2, history.Count);
		}

		[Test]
		public void UpdateNotLazyPropInOtherSession()
		{
			string notLazyStart = "notLazyBefore",
				lazyStart = "lazyNotUpd",
				notLazyEnd = "notLazyAfrer",
				lazyEnd = "lazyNotUpd";

			var factory = Cfg.BuildSessionFactory();

			using (var s = factory.OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.SaveOrUpdate(new EntityWithLazyProp
					{
						Id = 1,
						NotLazyProp = notLazyStart,
						LazyProp = lazyStart
					});
					tx.Commit();
				}
			}

			using (var s = factory.OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					var ent = s.Get<EntityWithLazyProp>(1);

					ent.NotLazyProp = notLazyEnd;

					s.SaveOrUpdate(ent);
					tx.Commit();
				}
			}

			var current = Session.Get<EntityWithLazyProp>(1);

			Assert.AreEqual(notLazyEnd, current.NotLazyProp);
			Assert.AreEqual(lazyEnd, current.LazyProp);

			var history = AuditReader()
				.CreateQuery()
				.ForRevisionsOfEntity(typeof(EntityWithLazyProp), false, true)
				.Add(AuditEntity.Id().Eq(1))
				.GetResultList();

			Assert.AreEqual(2, history.Count);
		}
	}
}
