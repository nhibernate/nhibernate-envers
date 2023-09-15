using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.CustomType;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.CustomType
{
	public partial class PrimitiveCustomTest : TestBase
	{
		private int pctec_id;

		public PrimitiveCustomTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.CustomType.Mapping.hbm.xml" }; }
		}

		protected override void Initialize()
		{
			var pctec = new PrimitiveCustomTypeEntity();

			using (var tx = Session.BeginTransaction())
			{
				pctec.PrimitiveType = PrimitiveImmutableType.Get(20);
				Session.Save(pctec);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				pctec.PrimitiveType = PrimitiveImmutableType.Get(21);
				tx.Commit();
			}
			pctec_id = pctec.Id;
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(PrimitiveCustomTypeEntity), pctec_id));
		}

		[Test]
		public void VerifyHistoryOfPctec()
		{
			var rev1 = AuditReader().Find<PrimitiveCustomTypeEntity>(pctec_id, 1);
			var rev2 = AuditReader().Find<PrimitiveCustomTypeEntity>(pctec_id, 2);

			Assert.AreEqual('u', rev1.PrimitiveType.CharValue);
			Assert.AreEqual('v', rev2.PrimitiveType.CharValue);
		}
	}
}
