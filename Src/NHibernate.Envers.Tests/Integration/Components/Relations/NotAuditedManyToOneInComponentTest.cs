using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.Components.Relations;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Components.Relations
{
	public partial class NotAuditedManyToOneInComponentTest : TestBase
	{
		private int mtocte_id1;

		public NotAuditedManyToOneInComponentTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Components.Relations.Mapping.hbm.xml", "Entities.Mapping.hbm.xml" }; }
		}

		protected override void Initialize()
		{
			var ste1 = new UnversionedStrTestEntity {Str = "str1"};
			var ste2 = new UnversionedStrTestEntity {Str = "str2"};
			var mtocte1 = new NotAuditedManyToOneComponentTestEntity { Comp1 = new NotAuditedManyToOneComponent { Data = "data1", Entity = ste1 } };
			using(var tx = Session.BeginTransaction())
			{
				Session.Save(ste1);
				Session.Save(ste2);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				mtocte_id1 = (int) Session.Save(mtocte1);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				mtocte1.Comp1.Data = "data2";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			CollectionAssert.AreEqual(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(NotAuditedManyToOneComponentTestEntity), mtocte_id1));
		}

		[Test]
		public void VerifyHistory()
		{
			var ver1 = new NotAuditedManyToOneComponentTestEntity
			           	{Id = mtocte_id1, Comp1 = new NotAuditedManyToOneComponent {Data = "data1"}};
			var ver2 = new NotAuditedManyToOneComponentTestEntity
			           	{Id = mtocte_id1, Comp1 = new NotAuditedManyToOneComponent {Data = "data2"}};

			Assert.AreEqual(ver1, AuditReader().Find<NotAuditedManyToOneComponentTestEntity>(mtocte_id1, 1));
			Assert.AreEqual(ver2, AuditReader().Find<NotAuditedManyToOneComponentTestEntity>(mtocte_id1, 2));
		}
	}
}