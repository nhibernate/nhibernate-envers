using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.OneToOne;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.OneToOne.BiDirectional
{
	public partial class BidirectionalEagerHbmTest : TestBase
	{
		private long refIngId1;

		public BidirectionalEagerHbmTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ed1 = new BidirectionalEagerHbmRefEdPK {Data = "data_ed_1"};
			var ing1 = new BidirectionalEagerHbmRefIngPK {Data = "data_ing_1", Reference = ed1};
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ed1);
				refIngId1 = (long) Session.Save(ing1);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyNonProxyObjectTraversing()
		{
			var referencing = AuditReader().Find<BidirectionalEagerHbmRefIngPK>(refIngId1, 1);
			referencing.Reference.Data.Should().Not.Be.Null();
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.OneToOne.EagerLoading.hbm.xml" };
			}
		}
	}
}