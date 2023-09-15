using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.CustomType;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.CustomType
{
	public partial class ParametrizedCustomTest : TestBase
	{
		private int pcte_id;

		public ParametrizedCustomTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.CustomType.Mapping.hbm.xml" }; }
		}

		protected override void Initialize()
		{
			var pcte = new ParametrizedCustomTypeEntity();

			using(var tx = Session.BeginTransaction())
			{
				pcte.Str = "U";
				Session.Save(pcte);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				pcte.Str = "V";
				tx.Commit();
			}
			pcte_id = pcte.Id;
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(ParametrizedCustomTypeEntity), pcte_id));
		}

		[Test]
		public void VerifyHistoryOfPcte()
		{
			var rev1 = AuditReader().Find<ParametrizedCustomTypeEntity>(pcte_id, 1);
			var rev2 = AuditReader().Find<ParametrizedCustomTypeEntity>(pcte_id, 2);

			Assert.AreEqual("xUy", rev1.Str);
			Assert.AreEqual("xVy", rev2.Str);
		}
	}
}