using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.CustomType;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.CustomType
{
	public partial class CompositeCustomTest : TestBase
	{
		private int ccte_id;

		public CompositeCustomTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.CustomType.Mapping.hbm.xml" }; }
		}

		protected override void Initialize()
		{
			var ccte = new CompositeCustomTypeEntity
			{
				Component = new Component { Prop1 = "a", Prop2 = 1 }
			};
			using(var tx = Session.BeginTransaction())
			{
				Session.Save(ccte);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				ccte.Component.Prop1 = "b";
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				ccte.Component = new Component {Prop1 = "c", Prop2 = 3};
				tx.Commit();
			}
			ccte_id = ccte.Id;
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, AuditReader().GetRevisions(typeof(CompositeCustomTypeEntity), ccte_id));
		}

		[Test]
		public void VerifyHistoryOfCccte()
		{
			var rev1 = AuditReader().Find<CompositeCustomTypeEntity>(ccte_id, 1);
			var rev2 = AuditReader().Find<CompositeCustomTypeEntity>(ccte_id, 2);
			var rev3 = AuditReader().Find<CompositeCustomTypeEntity>(ccte_id, 3);

			Assert.AreEqual(rev1.Component, new Component {Prop1 = "a", Prop2 = 1});
			Assert.AreEqual(rev2.Component, new Component {Prop1 = "b", Prop2 = 1});
			Assert.AreEqual(rev3.Component, new Component {Prop1 = "c", Prop2 = 3});
		}
	}
}