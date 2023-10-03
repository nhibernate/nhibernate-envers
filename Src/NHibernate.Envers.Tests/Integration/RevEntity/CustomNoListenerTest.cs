using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.RevEntity;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.RevEntity
{
	public partial class CustomNoListenerTest : TestBase
	{
		private int id;


		public CustomNoListenerTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml", "Entities.RevEntity.CustomDataRevEntity.hbm.xml" };
			}
		}

		protected override void Initialize()
		{
			var te = new StrTestEntity { Str = "x" };

			// Revision 1
			using (var tx = Session.BeginTransaction())
			{
				id = (int) Session.Save(te);

				// Setting the data on the revision entity
				var custom = AuditReader().GetCurrentRevision<CustomDataRevEntity>(false);
				custom.Data = "data1";
				tx.Commit();
			}
			// Revision 2
			using (var tx = Session.BeginTransaction())
			{
				te.Str = "y";

				// Setting the data on the revision entity
				var custom = (CustomDataRevEntity)AuditReader().GetCurrentRevision(false);
				custom.Data = "data2";
				tx.Commit();
			}
			// Revision 3 - no changes, but rev entity should be persisted
			using (var tx = Session.BeginTransaction())
			{
				// Setting the data on the revision entity
				var custom = AuditReader().GetCurrentRevision<CustomDataRevEntity>(true);
				custom.Data = "data3";
				tx.Commit();
			}
			// No changes, rev entity won't be persisted
			using (var tx = Session.BeginTransaction())
			{
				// Setting the data on the revision entity
				var custom = AuditReader().GetCurrentRevision<CustomDataRevEntity>(false);
				custom.Data = "data4";
				tx.Commit();
			}
			// Revision 4
			using (var tx = Session.BeginTransaction())
			{
				te.Str = "z";
				var custom = AuditReader().GetCurrentRevision<CustomDataRevEntity>(false);
				custom.Data = "data5";
				custom = AuditReader().GetCurrentRevision<CustomDataRevEntity>(false);
				custom.Data = "data5bis";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyFindRevision()
		{
			Assert.AreEqual("data1", AuditReader().FindRevision<CustomDataRevEntity>(1).Data);
			Assert.AreEqual("data2", AuditReader().FindRevision<CustomDataRevEntity>(2).Data);
			Assert.AreEqual("data3", AuditReader().FindRevision<CustomDataRevEntity>(3).Data);
			Assert.AreEqual("data5bis", AuditReader().FindRevision<CustomDataRevEntity>(4).Data);
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			CollectionAssert.AreEquivalent(new[]{1,2,4}, AuditReader().GetRevisions(typeof(StrTestEntity), id));
		}

		[Test]
		public void VerifyHistoryOfId()
		{
			var ver1 = new StrTestEntity {Id = id, Str = "x"};
			var ver2 = new StrTestEntity {Id = id, Str = "y"};
			var ver3 = new StrTestEntity {Id = id, Str = "z"};

			Assert.AreEqual(ver1, AuditReader().Find<StrTestEntity>(id, 1));
			Assert.AreEqual(ver2, AuditReader().Find<StrTestEntity>(id, 2));
			Assert.AreEqual(ver2, AuditReader().Find<StrTestEntity>(id, 3));
			Assert.AreEqual(ver3, AuditReader().Find<StrTestEntity>(id, 4));
		}
	}
}