using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Basic
{
	public partial class SimpleTest : TestBase
	{
		private int id1;

		public SimpleTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ite = new IntTestEntity { Number = 10 };
			using(var tx = Session.BeginTransaction())
			{
				id1 = (int) Session.Save(ite);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				ite.Number = 20;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(IntTestEntity), id1));
		}

		[Test]
		public void VerifyHistory()
		{
			var ver1 = new IntTestEntity { Id = id1, Number = 10 };
			var ver2 = new IntTestEntity { Id = id1, Number = 20 };

			Assert.AreEqual(ver1, AuditReader().Find<IntTestEntity>(id1, 1));
			Assert.AreEqual(ver2, AuditReader().Find(typeof(IntTestEntity),id1, 2));
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Mapping.hbm.xml" }; }
		}
	}
}