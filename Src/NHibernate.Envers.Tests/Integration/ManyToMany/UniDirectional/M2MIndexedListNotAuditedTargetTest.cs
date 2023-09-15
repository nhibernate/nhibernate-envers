using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.ManyToMany.UniDirectional;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ManyToMany.UniDirectional
{
	public partial class M2MIndexedListNotAuditedTargetTest :TestBase
	{
		private UnversionedStrTestEntity uste1;
		private UnversionedStrTestEntity uste2;
		private int itnae1_id;
		private int itnae2_id;

		public M2MIndexedListNotAuditedTargetTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml", "Entities.ManyToMany.UniDirectional.Mapping.hbm.xml" };
			}
		}

		protected override void Initialize()
		{
			uste1 = new UnversionedStrTestEntity {Str = "str1"};
			uste2 = new UnversionedStrTestEntity {Str = "str2"};

			//no revision
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(uste1);
				Session.Save(uste2);
				tx.Commit();
			}

			//Revision 1
			var itnae1 = new M2MIndexedListTargetNotAuditedEntity {Id = 1, Data = "tnae1"};
			using (var tx = Session.BeginTransaction())
			{
				itnae1.References.Add(uste1);
				itnae1.References.Add(uste2);
				itnae1_id = (int)Session.Save(itnae1);
				tx.Commit();
			}

			//Revision 2
			using (var tx = Session.BeginTransaction())
			{
				var itnae2 = new M2MIndexedListTargetNotAuditedEntity { Id = 2, Data = "tnae2" };
				itnae2.References.Add(uste2);
				itnae2_id = (int)Session.Save(itnae2);
				tx.Commit();
			}

			//Revision 3
			using (var tx = Session.BeginTransaction())
			{
				itnae1.References[0] = uste2;
				itnae1.References[1] = uste1;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionsCounts()
		{
			AuditReader().GetRevisions(typeof (M2MIndexedListTargetNotAuditedEntity), itnae1_id)
			             .Should().Have.SameSequenceAs(1, 3);
			AuditReader().GetRevisions(typeof (M2MIndexedListTargetNotAuditedEntity), itnae2_id)
			             .Should().Have.SameSequenceAs(2);
		}

		[Test]
		public void VerifyHistory1()
		{
			AuditReader().Find<M2MIndexedListTargetNotAuditedEntity>(itnae1_id, 1).References
			             .Should().Have.SameSequenceAs(uste1, uste2);
			AuditReader().Find<M2MIndexedListTargetNotAuditedEntity>(itnae1_id, 2).References
									 .Should().Have.SameSequenceAs(uste1, uste2);
			AuditReader().Find<M2MIndexedListTargetNotAuditedEntity>(itnae1_id, 3).References
									 .Should().Have.SameSequenceAs(uste2, uste1);
		}


		[Test]
		public void VerifyHistory2()
		{
			AuditReader().Find<M2MIndexedListTargetNotAuditedEntity>(itnae2_id, 1).Should().Be.Null();
			AuditReader().Find<M2MIndexedListTargetNotAuditedEntity>(itnae2_id, 2).References
									 .Should().Have.SameSequenceAs(uste2);
			AuditReader().Find<M2MIndexedListTargetNotAuditedEntity>(itnae2_id, 3).References
									 .Should().Have.SameSequenceAs(uste2);
		}
	}
}