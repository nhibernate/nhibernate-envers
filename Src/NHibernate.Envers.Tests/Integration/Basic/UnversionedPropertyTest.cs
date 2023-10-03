using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Basic
{
	public partial class UnversionedPropertyTest : TestBase
	{
		private int id1;

		public UnversionedPropertyTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Mapping.hbm.xml" }; }
		}

		protected override void Initialize()
		{
			var ue1 = new UnversionedEntity { Str1 = "a1", Str2 = "b1" };
			using(var tx = Session.BeginTransaction())
			{
				id1 = (int)Session.Save(ue1);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				ue1.Str1 = "a2";
				ue1.Str2 = "ab";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2}, AuditReader().GetRevisions(typeof(UnversionedEntity), id1));
		}

		[Test]
		public void VerifyHistoryOf1()
		{
			var ver1 = new UnversionedEntity { Id = id1, Str1 = "a1" };
			var ver2 = new UnversionedEntity { Id = id1, Str1 = "a2" };

			Assert.AreEqual(ver1, AuditReader().Find<UnversionedEntity>(id1, 1));
			Assert.AreEqual(ver2, AuditReader().Find<UnversionedEntity>(id1, 2));
		}
	}
}