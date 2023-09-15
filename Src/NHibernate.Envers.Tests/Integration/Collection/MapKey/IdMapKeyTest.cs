using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Collection.MapKey
{
	public partial class IdMapKeyTest : TestBase
	{
		private int imke_id;
		private int ste1_id;
		private int ste2_id;

		public IdMapKeyTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Integration.Collection.MapKey.Mapping.hbm.xml", "Entities.Mapping.hbm.xml", "Entities.Components.Mapping.hbm.xml" }; }
		}

		protected override void Initialize()
		{
			var imke = new IdMapKeyEntity();
			var ste1 = new StrTestEntity { Str = "x" };
			var ste2 = new StrTestEntity { Str = "y" };
			using(var tx = Session.BeginTransaction())
			{
				ste1_id = (int) Session.Save(ste1);
				ste2_id = (int)Session.Save(ste2);
				imke.IdMap[ste1.Id] = ste1;
				imke_id = (int)Session.Save(imke);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				imke.IdMap[ste2.Id] = ste2;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(IdMapKeyEntity), imke_id));
		}

		[Test]
		public void VerifyHistoryOfImke()
		{
			var ste1 = Session.Get<StrTestEntity>(ste1_id);
			var ste2 = Session.Get<StrTestEntity>(ste2_id);

			var rev1 = AuditReader().Find<IdMapKeyEntity>(imke_id, 1);
			var rev2 = AuditReader().Find<IdMapKeyEntity>(imke_id, 2);

			Assert.AreEqual(new Dictionary<int, StrTestEntity> { { ste1.Id, ste1 } }, rev1.IdMap);
			Assert.AreEqual(new Dictionary<int, StrTestEntity> { { ste1.Id, ste1 }, { ste2.Id, ste2 } }, rev2.IdMap);
		}
	}
}