using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.ManyToOne.UniDirectional;
using NHibernate.Proxy;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.ManyToOne.UniDirectional
{
	public partial class RelationNotAuditedTargetTest : TestBase
	{
		private int tnae1_id;
		private int tnae2_id;
		private int uste1_id;
		private int uste2_id;

		public RelationNotAuditedTargetTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			tnae1_id = 123;
			tnae2_id = 333;
			var uste1 = new UnversionedStrTestEntity { Str = "str1" };
			var uste2 = new UnversionedStrTestEntity { Str = "str2" };
			var tnae1 = new TargetNotAuditedEntity
			{
				Id = tnae1_id,
				Data = "tnae1",
				Reference = uste1
			};
			var tnae2 = new TargetNotAuditedEntity
			{
				Id = tnae2_id,
				Data = "tnae2",
				Reference = uste2
			};


			// No revision
			using (var tx = Session.BeginTransaction())
			{
				uste1_id = (int)Session.Save(uste1);
				uste2_id = (int)Session.Save(uste2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(tnae1);
				Session.Save(tnae2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				tnae1.Reference = uste2;
				tnae2.Reference = uste1;
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				tnae1.Reference = uste2;
				tnae2.Reference = uste2;
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				tnae1.Reference = uste1;
				tnae2.Reference = uste1;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 4 }, AuditReader().GetRevisions(typeof(TargetNotAuditedEntity), tnae1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4 }, AuditReader().GetRevisions(typeof(TargetNotAuditedEntity), tnae2_id));
		}

		[Test]
		public void VerifyHistoryNae1()
		{
			// load original "tnae1" TargetNotAuditedEntity to force load "str1" UnversionedStrTestEntity as Proxy
			var original = Session.Get<TargetNotAuditedEntity>(tnae1_id);

			var uste1 = Session.Get<UnversionedStrTestEntity>(uste1_id);
			var uste2 = Session.Get<UnversionedStrTestEntity>(uste2_id);

			var rev1 = AuditReader().Find<TargetNotAuditedEntity>(tnae1_id, 1);
			var rev2 = AuditReader().Find<TargetNotAuditedEntity>(tnae1_id, 2);
			var rev3 = AuditReader().Find<TargetNotAuditedEntity>(tnae1_id, 3);
			var rev4 = AuditReader().Find<TargetNotAuditedEntity>(tnae1_id, 4);

			Assert.AreEqual(uste1, rev1.Reference);
			Assert.AreEqual(uste2, rev2.Reference);
			Assert.AreEqual(uste2, rev3.Reference);
			Assert.AreEqual(uste1, rev4.Reference);

			Assert.IsTrue(original.Reference is INHibernateProxy);
			Assert.AreEqual(typeof(UnversionedStrTestEntity), NHibernateUtil.GetClass(original.Reference));
			Assert.AreEqual(typeof(UnversionedStrTestEntity), NHibernateProxyHelper.GetClassWithoutInitializingProxy(rev1.Reference));
			Assert.AreEqual(typeof(UnversionedStrTestEntity), NHibernateUtil.GetClass(rev1.Reference));
		}

		[Test]
		public void VerifyHistoryNae2()
		{
			var uste1 = Session.Get<UnversionedStrTestEntity>(uste1_id);
			var uste2 = Session.Get<UnversionedStrTestEntity>(uste2_id);

			var rev1 = AuditReader().Find<TargetNotAuditedEntity>(tnae2_id, 1);
			var rev2 = AuditReader().Find<TargetNotAuditedEntity>(tnae2_id, 2);
			var rev3 = AuditReader().Find<TargetNotAuditedEntity>(tnae2_id, 3);
			var rev4 = AuditReader().Find<TargetNotAuditedEntity>(tnae2_id, 4);

			Assert.AreEqual(uste2, rev1.Reference);
			Assert.AreEqual(uste1, rev2.Reference);
			Assert.AreEqual(uste2, rev3.Reference);
			Assert.AreEqual(uste1, rev4.Reference);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml", "Entities.ManyToOne.UniDirectional.Mapping.hbm.xml" };
			}
		}
	}
}