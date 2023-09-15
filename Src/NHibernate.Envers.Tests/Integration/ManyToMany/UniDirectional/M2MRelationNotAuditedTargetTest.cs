using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.ManyToMany.UniDirectional;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.ManyToMany.UniDirectional
{
	public partial class M2MRelationNotAuditedTargetTest : TestBase
	{
		private int tnae1_id;
		private int tnae2_id;
		private int uste1_id;
		private int uste2_id;

		public M2MRelationNotAuditedTargetTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			tnae1_id = 123;
			tnae2_id = 333;
			var uste1 = new UnversionedStrTestEntity {Str = "str1"};
			var uste2 = new UnversionedStrTestEntity {Str = "str2"};
			var tnae1 = new M2MTargetNotAuditedEntity
			{
				Id = tnae1_id,
				Data = "tnae1",
				References = new List<UnversionedStrTestEntity>()
			};
			var tnae2 = new M2MTargetNotAuditedEntity
			{
				Id = tnae2_id,
				Data = "tnae2",
				References = new List<UnversionedStrTestEntity>()
			};


			// No revision
			using (var tx = Session.BeginTransaction())
			{
				uste1_id = (int) Session.Save(uste1);
				uste2_id = (int) Session.Save(uste2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				tnae2.References.Add(uste1);
				tnae2.References.Add(uste2);
				Session.Save(tnae1);
				Session.Save(tnae2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				tnae1.References.Add(uste1);
				tnae2.References.Remove(uste1);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				tnae1.References.Add(uste1);
				tnae2.References.Remove(uste2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				tnae1.References.Add(uste2);
				tnae2.References.Add(uste1);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 4 }, AuditReader().GetRevisions(typeof(M2MTargetNotAuditedEntity), tnae1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4 }, AuditReader().GetRevisions(typeof(M2MTargetNotAuditedEntity), tnae2_id));
		}


		[Test]
		public void VerifyHistoryNae1()
		{
			var uste1 = Session.Get<UnversionedStrTestEntity>(uste1_id);
			var uste2 = Session.Get<UnversionedStrTestEntity>(uste2_id);

			var rev1 = AuditReader().Find<M2MTargetNotAuditedEntity>(tnae1_id, 1);
			var rev2 = AuditReader().Find<M2MTargetNotAuditedEntity>(tnae1_id, 2);
			var rev3 = AuditReader().Find<M2MTargetNotAuditedEntity>(tnae1_id, 3);
			var rev4 = AuditReader().Find<M2MTargetNotAuditedEntity>(tnae1_id, 4);

			CollectionAssert.IsEmpty(rev1.References);
			CollectionAssert.AreEquivalent(new[] { uste1 }, rev2.References);
			CollectionAssert.AreEquivalent(new[] { uste1 }, rev3.References);
			CollectionAssert.AreEquivalent(new[] { uste1, uste2 }, rev4.References);
		}

		[Test]
		public void VerifyHistoryNae2()
		{
			var uste1 = Session.Get<UnversionedStrTestEntity>(uste1_id);
			var uste2 = Session.Get<UnversionedStrTestEntity>(uste2_id);

			var rev1 = AuditReader().Find<M2MTargetNotAuditedEntity>(tnae2_id, 1);
			var rev2 = AuditReader().Find<M2MTargetNotAuditedEntity>(tnae2_id, 2);
			var rev3 = AuditReader().Find<M2MTargetNotAuditedEntity>(tnae2_id, 3);
			var rev4 = AuditReader().Find<M2MTargetNotAuditedEntity>(tnae2_id, 4);


			CollectionAssert.AreEquivalent(new[] { uste1, uste2 }, rev1.References);
			CollectionAssert.AreEquivalent(new[] { uste2 }, rev2.References);
			CollectionAssert.IsEmpty(rev3.References);
			CollectionAssert.AreEquivalent(new[] { uste1 }, rev4.References);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml", "Entities.ManyToMany.UniDirectional.Mapping.hbm.xml" };
			}
		}
	}
}