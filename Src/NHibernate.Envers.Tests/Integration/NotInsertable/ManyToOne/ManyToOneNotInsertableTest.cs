using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.NotInsertable.ManyToOne
{
	public partial class ManyToOneNotInsertableTest : TestBase
	{
		private int mto_id1;
		private int type_id1;
		private int type_id2;

		public ManyToOneNotInsertableTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			mto_id1 = 1;
			type_id1 = 2;
			type_id2 = 3;

			var type1 = new NotInsertableEntityType { TypeId = type_id1, Type = "type1" };
			var type2 = new NotInsertableEntityType { TypeId = type_id2, Type = "type2" };
			var master = new ManyToOneNotInsertableEntity { Id = mto_id1, Number = type_id1, Type = type1 };

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(type1);
				Session.Save(type2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(master);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				master.Number = type_id2;
				tx.Commit();
			}
		}


		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(NotInsertableEntityType), type_id1));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(NotInsertableEntityType), type_id2));
			CollectionAssert.AreEquivalent(new[] { 2, 3 }, AuditReader().GetRevisions(typeof(ManyToOneNotInsertableEntity), mto_id1));
		}

		[Test]
		public void VerifyHistory()
		{
			var ver1 = AuditReader().Find<ManyToOneNotInsertableEntity>(mto_id1, 1);
			var ver2 = AuditReader().Find<ManyToOneNotInsertableEntity>(mto_id1, 2);
			var ver3 = AuditReader().Find<ManyToOneNotInsertableEntity>(mto_id1, 3);

			var type1 = Session.Get<NotInsertableEntityType>(type_id1);
			var type2 = Session.Get<NotInsertableEntityType>(type_id2);

			Assert.IsNull(ver1);
			Assert.AreEqual(type1, ver2.Type);
			Assert.AreEqual(type2, ver3.Type);
		}
	}
}