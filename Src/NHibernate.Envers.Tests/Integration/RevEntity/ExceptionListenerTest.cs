using System;
using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.RevEntity
{
	public partial class ExceptionListenerTest : TestBase
	{
		public ExceptionListenerTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml", "Entities.RevEntity.ExceptionListenerRevEntity.hbm.xml" };
			}
		}

		protected override void Initialize()
		{
			// Trying to persist an entity - however the listener should throw an exception, so the entity
			// shouldn't be persisted
			var te = new StrTestEntity { Str = "x" };
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(te);
				try
				{
					tx.Commit();
				}
				catch (Exception)
				{
					//just eat the exception
				}
			}
		}

		[Test]
		public void EntityShouldNotHaveBeenPersisted()
		{
			Assert.AreEqual(0, Session.CreateQuery("select count(s) from StrTestEntity s where s.Str='x'").UniqueResult());
		}
	}
}