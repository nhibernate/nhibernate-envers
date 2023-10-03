using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.OneToMany.Detached;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToMany.Detached
{
	public partial class DataChangesDetachedSetTest : TestBase
	{
		private int str1_id;
		private int coll1_id;

		public DataChangesDetachedSetTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			coll1_id = 1234;
			var str1 = new StrTestEntity {Str = "str1"};
			var coll1 = new SetRefCollEntity {Id = coll1_id, Data = "coll1"};
			using (var tx = Session.BeginTransaction())
			{
				str1_id = (int)Session.Save(str1);
				coll1.Collection = new HashSet<StrTestEntity>();
				Session.Save(coll1);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				coll1.Collection.Add(str1);
				coll1.Data = "coll2";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(SetRefCollEntity), coll1_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(StrTestEntity), str1_id));
		}


		[Test]
		public void VerifyHistoryOfColl1()
		{
			var str1 = Session.Get<StrTestEntity>(str1_id);

			var rev1 = AuditReader().Find<SetRefCollEntity>(coll1_id, 1);
			var rev2 = AuditReader().Find<SetRefCollEntity>(coll1_id, 2);

			CollectionAssert.IsEmpty(rev1.Collection);
			CollectionAssert.AreEquivalent(new[] { str1 }, rev2.Collection);

			Assert.AreEqual("coll1", rev1.Data);
			Assert.AreEqual("coll2", rev2.Data);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml", "Entities.OneToMany.Detached.Mapping.hbm.xml" };
			}
		}
	}
}